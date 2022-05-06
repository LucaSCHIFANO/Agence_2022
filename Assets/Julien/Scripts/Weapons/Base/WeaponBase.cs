using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

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


    #endregion
    
    
    // New overheat system
    protected float _shootingTimer;
    protected float _timeCoolDown;
    public bool _isOverHeat;
    public bool _isCoolDown;
    
    [SerializeField][Range(0, 100)] private float overHeatPourcent;

    protected virtual void Start()
    {
        overHeatPourcent = 0;
    }

    protected virtual void FixedUpdate()
    {
        if (_shootingTimer >= 0) _shootingTimer -= Time.deltaTime;
    }

    public virtual void Shoot()
    {
        overHeatPourcent += (100 / _bulletToOverHeat);
        if (overHeatPourcent >= 100) _isOverHeat = true;
        _timeCoolDown = _timeBeforeCoolDown;
        
        EventSystem.ShootEvent();
    }

    public void Reload()
    {
        if (_isOverHeat) return;
        
        _isOverHeat = true;
    }

    private void Update()
    {
        if (_isCoolDown || _isOverHeat) overHeatPourcent -= _coolDownPerSecond * Time.deltaTime;
        else if(!_isCoolDown) _timeCoolDown -= Time.deltaTime;

        if (_timeCoolDown <= 0) _isCoolDown = true;
        else _isCoolDown = false;
        if (overHeatPourcent <= 0) _isOverHeat = false;

        overHeatPourcent = Mathf.Clamp(overHeatPourcent, 0, 100);
    }
}