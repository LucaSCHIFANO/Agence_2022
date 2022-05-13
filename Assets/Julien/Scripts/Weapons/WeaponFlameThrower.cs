using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;

public class WeaponFlameThrower : WeaponBase
{
    [Header("Flamethrower Config")] [SerializeField]
    //private ParticleSystem _particleSystem;
    public GameObject particleFire;
    public override void Shoot()
    {
        if (_shootingTimer > 0) return;
        if (_isOverHeat) return;
        base.Shoot();

        //_particleSystem.Play();
        ShootBulletServerRpc();
        Instantiate(particleFire, _shootingPoint.position, _shootingPoint.rotation);

    }
    
    [ClientRpc(Delivery = RpcDelivery.Unreliable)]
    protected override void ShootBulletClientRpc()
    {
        if(IsOwner) return;
        Instantiate(particleFire, _shootingPoint.position, _shootingPoint.rotation);
        
    }
}
