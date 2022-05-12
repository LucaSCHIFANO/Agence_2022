using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;

public class WeaponTesla : WeaponBase
{
    public override void Shoot()
    {
        if (_shootingTimer > 0) return;
        if (_isOverHeat) return;
        
        base.Shoot();
        
        ShootProjectileServerRpc();
        
        _shootingTimer = 1 / _fireRate;
    }
    
    
    [ServerRpc]
    void ShootProjectileServerRpc()
    {
        GameObject bulletGO = Instantiate(_bulletPrefab, _shootingPoint.position, _shootingPoint.rotation);
        bulletGO.GetComponent<NetworkObject>().Spawn();
    }
}
