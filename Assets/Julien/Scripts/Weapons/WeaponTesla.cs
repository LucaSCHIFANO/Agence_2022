using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WeaponTesla : WeaponBase
{
    public override void Shoot()
    {
        if (_shootingTimer > 0) return;
        if (_isOverHeat) return;
        
        base.Shoot();
        
        GameObject bulletGO = Instantiate(_bulletPrefab, _shootingPoint.position, _shootingPoint.rotation);
        
        _shootingTimer = 1 / _fireRate;
    }
}
