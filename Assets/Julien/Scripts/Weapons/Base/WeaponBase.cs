using System;
using System.Collections;
using System.Threading.Tasks;
using Fusion;
using Game;
using UnityEngine;
using UnityEngine.UI;

public abstract class WeaponBase : NetworkBehaviour
{
    #region Exposed Variables

    [Header("General Weapon Config")]

    [Tooltip("The max number of shoot in one second")]
    [SerializeField]
    protected float _fireRate = 1;

    [Tooltip("The point from where the bullet / ray is shoot")]
    [SerializeField]
    protected Transform _shootingPoint;

    [Tooltip("The prefab of the bullet to shoot")]
    [SerializeField]
    protected GameObject _bulletPrefab;

    
    [SerializeField] protected float _bulletToOverHeat;
    
    [Tooltip("Pourcent of overheat gauge remove per second")]
    [SerializeField] protected float _coolDownPerSecond;
    [SerializeField] protected float _timeBeforeCoolDown;

    [SerializeField] protected Color maincolor;
    [SerializeField] protected Color overHeatColor;

    [SerializeField] protected GameObject bulletEffect;

    [SerializeField] protected ParticleSystem overHParticle;
    [SerializeField] protected float OHParticleOverTime;
    [SerializeField] protected bool allieTouret;


    #endregion
    
    
    // New overheat system
    protected float _shootingTimer;
    protected float _timeCoolDown;
    protected bool _isOverHeat;
    protected bool _isCoolDown;
    protected bool _isDisable;

    [HideInInspector] public NetworkedPlayer possessor;

    [HideInInspector] public bool isPossessed;
    
    public CanvasInGame canvas;


    public float overHeatPourcent;
    [Networked(OnChanged = nameof(OnOverHChanged))] protected float overHeatPourcentOnline{ get; set; }

    public override void Spawned()
    {
        base.Spawned();
        overHeatPourcentOnline = 0;
        isPossessed = false;
    }
    
    public override void FixedUpdateNetwork()
    {
        if (_shootingTimer >= 0) _shootingTimer -= Time.deltaTime;
    }

    public static void OnOverHChanged(Changed<WeaponBase> changed)
    {
        changed.Behaviour.ChangeOverHeat();
    }
    
    public void ChangeOverHeat()
    {
        overHeatPourcent = overHeatPourcentOnline;
        if (overHeatPourcent >= 100) _isOverHeat = true;


        if (Object.HasInputAuthority)
        {
            canvas.overheatSlider.fillAmount = overHeatPourcent / 100f;
            if (_isOverHeat) canvas.overheatSlider.color = overHeatColor;
            else canvas.overheatSlider.color = maincolor;
        }
    }

    public virtual void Shoot()
    {
        if (Object == null) return;
        if (Runner.IsServer)
        {
            overHeatPourcent += (100 / _bulletToOverHeat);
            overHeatPourcentOnline = overHeatPourcent;
        
            if (overHeatPourcent >= 100) _isOverHeat = true;
            _timeCoolDown = _timeBeforeCoolDown;
        
            EventSystem.ShootEvent();
        }
    }

    public void Reload()
    {
        if (_isOverHeat) return;
        
        _isOverHeat = true;
    }

    public void DisableWeapon()
    {
        if (Object == null) return;
        _isDisable = true;
        _isOverHeat = true;
        _coolDownPerSecond = 0;
        overHeatPourcentOnline = 100f;
        _isCoolDown = false;
        
        var em = overHParticle.emission;
        em.enabled = true;
        em.rateOverTime = OHParticleOverTime;
    }

    private void Update()
    {
        if (Runner != null)
        {
            if (Runner.IsServer)
            {
                if (_isCoolDown || _isOverHeat) overHeatPourcent -= _coolDownPerSecond * Time.deltaTime;
                else if (!_isCoolDown && _timeCoolDown > 0) _timeCoolDown -= Time.deltaTime;
                
                if (_timeCoolDown <= 0) _isCoolDown = true;
                else _isCoolDown = false;
        
                if (overHeatPourcent <= 0) _isOverHeat = false;
                
                overHeatPourcent = Mathf.Clamp(overHeatPourcent, 0, 100);
                
                overHeatPourcentOnline = overHeatPourcent;
            }
        }
        
        var em = overHParticle.emission;
        em.enabled = true;

        if (_isOverHeat) em.rateOverTime = OHParticleOverTime;
        else em.rateOverTime = 0f;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void AskToShoot()
    {
        Shoot();
    }
    
    
    //creer une particule qd une bullet touche un mur
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void BulletEffectClientRpc(Vector3 impactPoint)
    {
        // if(IsOwner) return;
        Instantiate(bulletEffect, impactPoint, transform.rotation);
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    protected void CreateBulletEffectServerRpc(Vector3 impactPoint)
    {
        BulletEffectClientRpc(impactPoint);
    }
    
    
    //creer la balle override en fct de l'arme
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected virtual void ShootBulletClientRpc()
    {
        // if(IsOwner) return;
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    protected void ShootBulletServerRpc()
    {
        ShootBulletClientRpc();
    }
}