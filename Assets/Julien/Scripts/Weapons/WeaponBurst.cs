using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.Netcode;

public class WeaponBurst : WeaponBase
{
    [Header("Burst Config")]
    
    [SerializeField] private WeaponFireType _fireType;

    [SerializeField] private float _spread;
    
    [SerializeField] private int _numberOfShot;


    private int shootedRound;
    private bool isShooting;
    
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isShooting)
        {
            Shoot();
        }
    }

    public override void Shoot()
    {
        isShooting = true;
        if (_shootingTimer > 0) return;
        if (_isOverHeat) return;
        
        base.Shoot();
        
        if (_fireType == WeaponFireType.Hitscan)
        {
            RaycastHit hit;
            Vector3 shootingDir = Quaternion.Euler(Random.Range(-_spread, _spread), Random.Range(-_spread, _spread), Random.Range(-_spread, _spread)) * _shootingPoint.forward;
            Debug.DrawRay(_shootingPoint.position, shootingDir * 1000, Color.red, 10);
            if (Physics.Raycast(_shootingPoint.position, shootingDir, out hit))
            {
                // Toucher un collider
            }
        }
        else if (_fireType == WeaponFireType.Projectile)
        {
            // (Modifier cette ligne si object pooling)
            ShootProjectileServerRpc();
        }
        shootedRound++;
        _shootingTimer = .1f;

        if (shootedRound >= _numberOfShot)
        {
            shootedRound = 0;
            _shootingTimer = 1 / _fireRate;
            isShooting = false;
        }
    }
    
    
    
    [ServerRpc]
    void ShootProjectileServerRpc()
    {
        GameObject bulletGO = Instantiate(_bulletPrefab, _shootingPoint.position, _shootingPoint.rotation * Quaternion.Euler(new Vector3(Random.Range(-_spread, _spread),
            Random.Range(-_spread, _spread), Random.Range(-_spread, _spread))));
        
        bulletGO.GetComponent<NetworkObject>().Spawn();
    }
    
}
