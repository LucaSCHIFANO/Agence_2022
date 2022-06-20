using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;

public class WeaponTesla : WeaponBase
{
    public override void Shoot()
    {
        if (_shootingTimer > 0) return;
        if (_isOverHeat) return;
        
        base.Shoot();
        
        //ShootProjectileServerRpc();
        ShootBulletServerRpc();
        GameObject bulletGO = Instantiate(_bulletPrefab, _shootingPoint.position, _shootingPoint.rotation);
        
        _shootingTimer = 1 / _fireRate;
    }
    
    
    [Rpc]
    void ShootProjectileServerRpc()
    {
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected override void ShootBulletClientRpc()
    {
        Instantiate(_bulletPrefab, _shootingPoint.position, _shootingPoint.rotation);
    }
}
