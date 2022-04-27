using System;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
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

    #endregion

    protected float shootingTimer;

    protected virtual void FixedUpdate()
    {
        if (shootingTimer >= 0) shootingTimer -= Time.deltaTime;
    }

    public virtual void Shoot()
    {
        EventSystem.ShootEvent();
    }
}