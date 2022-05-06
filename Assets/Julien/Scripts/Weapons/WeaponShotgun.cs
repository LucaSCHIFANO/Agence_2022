using System.Threading.Tasks;
using UnityEngine;

public class WeaponShotgun : WeaponBase
{
    [Header("Shotgun config")] [SerializeField]
    private int _numberOfBullet;

    [SerializeField] private float _spread;

    public override void Shoot()
    {
        if (_shootingTimer > 0) return;
        if (_isOverHeat) return;
        
        base.Shoot();
        
        // (Modifier cette ligne si object pooling)
        for (int i = 0; i < _numberOfBullet; i++)
        {
            GameObject bulletGO = Instantiate(_bulletPrefab, _shootingPoint.position,
                _shootingPoint.rotation * Quaternion.Euler(new Vector3(Random.Range(-_spread, _spread),
                    Random.Range(-_spread, _spread), Random.Range(-_spread, _spread))));
        }
        
        _shootingTimer = 1 / _fireRate;
    }
}