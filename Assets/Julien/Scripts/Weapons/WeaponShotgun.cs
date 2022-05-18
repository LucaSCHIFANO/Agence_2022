using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;

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
        
        //ShootProjectileServerRpc();
        ShootBulletServerRpc();
        for (int i = 0; i < _numberOfBullet; i++)
        {
            GameObject bulletGO = Instantiate(_bulletPrefab, _shootingPoint.position,
                _shootingPoint.rotation * Quaternion.Euler(new Vector3(Random.Range(-_spread, _spread),
                    Random.Range(-_spread, _spread), Random.Range(-_spread, _spread))));
        }
        
        _shootingTimer = 1 / _fireRate;
    }
    
    
    
    [ServerRpc]
    void ShootProjectileServerRpc()
    {
        // (Modifier cette ligne si object pooling)
        for (int i = 0; i < _numberOfBullet; i++)
        {
            GameObject bulletGO = Instantiate(_bulletPrefab, _shootingPoint.position,
                _shootingPoint.rotation * Quaternion.Euler(new Vector3(Random.Range(-_spread, _spread),
                    Random.Range(-_spread, _spread), Random.Range(-_spread, _spread))));
            bulletGO.GetComponent<NetworkObject>().Spawn();
        }
    }
    
    [ClientRpc(Delivery = RpcDelivery.Unreliable)]
    protected override void ShootBulletClientRpc()
    {
        if(IsOwner) return;
        for (int i = 0; i < _numberOfBullet; i++)
        {
            GameObject bulletGO = Instantiate(_bulletPrefab, _shootingPoint.position,
                _shootingPoint.rotation * Quaternion.Euler(new Vector3(Random.Range(-_spread, _spread),
                    Random.Range(-_spread, _spread), Random.Range(-_spread, _spread))));
        }
        
    }
}