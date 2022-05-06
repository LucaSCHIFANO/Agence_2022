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

    [SerializeField] protected int magazineSize;
    [SerializeField] protected float reloadTime;

    #endregion

    protected int bulletLeft;
    protected bool isReloading;
    protected float shootingTimer;

    protected virtual void Start()
    {
        bulletLeft = magazineSize;
    }

    protected virtual void FixedUpdate()
    {
        if (shootingTimer >= 0) shootingTimer -= Time.deltaTime;
    }

    public virtual void Shoot()
    {
        bulletLeft--;
        
        EventSystem.ShootEvent();
    }

    public void Reload()
    {
        if (isReloading) return;
        
        isReloading = true;
        StartCoroutine(ReloadCoroutine());
    }

    IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSecondsRealtime(reloadTime);

        bulletLeft = magazineSize;
        isReloading = false;
    }
    
    
}