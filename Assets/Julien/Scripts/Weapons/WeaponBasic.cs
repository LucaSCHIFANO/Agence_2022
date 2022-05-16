using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.Netcode;

public class WeaponBasic : WeaponBase
{
    [Header("Burst Config")]
    
    [SerializeField] public WeaponFireType _fireType;

    [SerializeField] public float _spread;
    
    [SerializeField] public int _numberOfShot;


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
    
    
    
    [ServerRpc]
    void ShootProjectileServerRpc()
    {
        GameObject bulletGO = Instantiate(_bulletPrefab, _shootingPoint.position, _shootingPoint.rotation * Quaternion.Euler(new Vector3(Random.Range(-_spread, _spread),
            Random.Range(-_spread, _spread), Random.Range(-_spread, _spread))));
        
        bulletGO.GetComponent<NetworkObject>().Spawn();
    }
    
    
    [ClientRpc(Delivery = RpcDelivery.Unreliable)]
    protected override void ShootBulletClientRpc()
    {
        if(IsOwner) return;
        GameObject bulletGO = Instantiate(_bulletPrefab, _shootingPoint.position, _shootingPoint.rotation * Quaternion.Euler(new Vector3(Random.Range(-_spread, _spread),
            Random.Range(-_spread, _spread), Random.Range(-_spread, _spread))));
        
    }
    
}
