using System.Threading.Tasks;
using Fusion;
using UnityEngine;

public class WeaponMachineGun : WeaponBase
{
    [Header("Machine Gun Config")]
    
    [SerializeField] public WeaponFireType _fireType;

    [SerializeField] public float _spread;
    

    public override void Shoot()
    {
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
                CreateBulletEffectServerRpc(hit.point, hit.collider.tag);
                Instantiate(bulletEffectSand, hit.point, transform.rotation);
            }
        }
        else if (_fireType == WeaponFireType.Projectile)
        {
            // (Modifier cette ligne si object pooling)
            ShootBulletServerRpc();
            GameObject bulletGO = Instantiate(_bulletPrefab, _shootingPoint.position, _shootingPoint.rotation * Quaternion.Euler(new Vector3(Random.Range(-_spread, _spread),
                Random.Range(-_spread, _spread), Random.Range(-_spread, _spread))));
        }
        
        _shootingTimer = 1 / _fireRate;
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
        Instantiate(_bulletPrefab, _shootingPoint.position, _shootingPoint.rotation * Quaternion.Euler(new Vector3(Random.Range(-_spread, _spread),
            Random.Range(-_spread, _spread), Random.Range(-_spread, _spread))));
        
    }
    
    
}