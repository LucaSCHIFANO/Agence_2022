using System.Threading.Tasks;
using Fusion;
using UnityEngine;

public class WeaponSniper : WeaponBase
{
    [Header("Sniper Config")]
    
    [SerializeField] protected WeaponFireType _fireType;

    [SerializeField] protected float _spread;
    
    public override void Shoot()
    {
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
                CreateBulletEffectServerRpc(hit.point);
                Instantiate(bulletEffect, hit.point, transform.rotation);
            }
        }
        else if (_fireType == WeaponFireType.Projectile)
        {
            // (Modifier cette ligne si object pooling)
            ShootBulletServerRpc();
            GameObject bulletGO = Instantiate(_bulletPrefab, _shootingPoint.position, _shootingPoint.rotation);
        }
        
        _shootingTimer = 1 / _fireRate;
    }
    
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected override void ShootBulletClientRpc()
    {
        Instantiate(_bulletPrefab, _shootingPoint.position, _shootingPoint.rotation);
        
    }
    
}
