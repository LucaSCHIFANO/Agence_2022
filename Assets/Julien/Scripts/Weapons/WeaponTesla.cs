using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WeaponTesla : WeaponBase
{
    public override void Shoot()
    {
        if (shootingTimer > 0) return;
        if (bulletLeft <= 0) return;
        
        base.Shoot();
        
        GameObject bulletGO = Instantiate(_bulletPrefab, _shootingPoint.position, _shootingPoint.rotation);
        
        shootingTimer = 1 / _fireRate;
    }
}
