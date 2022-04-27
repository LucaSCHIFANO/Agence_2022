using UnityEngine;

public class WeaponMachineGun : WeaponBase
{
    [Header("Machine Gun Config")]
    
    [SerializeField] private WeaponFireType _fireType;

    [SerializeField] private float _spread;

    public override void Shoot()
    {
        if (shootingTimer > 0) return;
        
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
            GameObject bulletGO = Instantiate(_bulletPrefab, _shootingPoint.position, _shootingPoint.rotation * Quaternion.Euler(new Vector3(Random.Range(-_spread, _spread),
                Random.Range(-_spread, _spread), Random.Range(-_spread, _spread))));
        }
        
        shootingTimer = 1 / _fireRate;
        
        base.Shoot();
    }
}