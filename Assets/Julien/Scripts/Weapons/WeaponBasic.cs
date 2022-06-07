using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponBasic : WeaponBase
{
    [Header("Burst Config")]
    
    [SerializeField] protected WeaponFireType _fireType;

    [SerializeField] protected float _spread;
    
    [SerializeField] protected int _numberOfShot;


    private int shootedRound;
    private bool isShooting;
    
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

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
            Debug.DrawRay(_shootingPoint.position, shootingDir * 1000, maincolor, 1);
            if (Physics.Raycast(_shootingPoint.position, shootingDir, out hit))
            {
                CreateBulletEffectServerRpc(hit.point);
                Instantiate(bulletEffect, hit.point, transform.rotation);
            }
        }
        else if (_fireType == WeaponFireType.Projectile)
        {
            // (Modifier cette ligne si object pooling)
            //ShootProjectileServerRpc();
            ShootBulletServerRpc();
            GameObject bulletGO = Instantiate(_bulletPrefab, _shootingPoint.position, _shootingPoint.rotation * Quaternion.Euler(new Vector3(Random.Range(-_spread, _spread),
                Random.Range(-_spread, _spread), Random.Range(-_spread, _spread))));
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
    
    
    
    /*[ServerRpc]
    void ShootProjectileServerRpc()
    {
        GameObject bulletGO = Instantiate(_bulletPrefab, _shootingPoint.position, _shootingPoint.rotation * Quaternion.Euler(new Vector3(Random.Range(-_spread, _spread),
            Random.Range(-_spread, _spread), Random.Range(-_spread, _spread))));
        
        bulletGO.GetComponent<NetworkObject>().Spawn();
    }*/
    
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected override void ShootBulletClientRpc()
    {
        // if(IsOwner) return;
        Instantiate(_bulletPrefab, _shootingPoint.position, _shootingPoint.rotation * Quaternion.Euler(new Vector3(Random.Range(-_spread, _spread),
            Random.Range(-_spread, _spread), Random.Range(-_spread, _spread))));
        
    }
    
}
