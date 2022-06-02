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


    #endregion
    
    
    // New overheat system
    protected float _shootingTimer;
    protected float _timeCoolDown;
    protected bool _isOverHeat;
    protected bool _isCoolDown;

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
    
    private void ChangeOverHeat()
    {
        overHeatPourcent = overHeatPourcentOnline;
        if (overHeatPourcent >= 100) _isOverHeat = true;

    }

    public virtual void Shoot()
    {
        overHeatPourcent += (100 / _bulletToOverHeat);
        overHeatPourcentOnline = overHeatPourcent;
        
        if (overHeatPourcent >= 100) _isOverHeat = true;
        _timeCoolDown = _timeBeforeCoolDown;
        
        EventSystem.ShootEvent();
    }

    public void Reload()
    {
        if (_isOverHeat) return;
        
        _isOverHeat = true;
    }

    public void DisableWeapon()
    {
        _isOverHeat = true;
        _coolDownPerSecond = 0;
        overHeatPourcentOnline = 100f;
        _isCoolDown = false;
    }

    private void Update()
    {
        if (_isCoolDown || _isOverHeat) overHeatPourcent -= _coolDownPerSecond * Time.deltaTime;
        else if(!_isCoolDown) _timeCoolDown -= Time.deltaTime;

        if (_timeCoolDown <= 0) _isCoolDown = true;
        else _isCoolDown = false;
        
        if (overHeatPourcent <= 0) _isOverHeat = false;

        var em = overHParticle.emission;
        em.enabled = true;

        if (_isOverHeat) em.rateOverTime = OHParticleOverTime;
        else em.rateOverTime = 0f;
    

        overHeatPourcent = Mathf.Clamp(overHeatPourcent, 0, 100);





        if (Object != null) overHeatPourcentOnline = overHeatPourcent;
        

        Debug.Log(canvas.name+ "  " + canvas.overheatSlider.name + " " + overHeatPourcent);
        canvas.overheatSlider.fillAmount = (overHeatPourcent / 100f);
        if (_isOverHeat) canvas.overheatSlider.color = overHeatColor;
        else canvas.overheatSlider.color = maincolor;
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