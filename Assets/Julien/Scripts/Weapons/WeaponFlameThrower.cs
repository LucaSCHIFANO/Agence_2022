using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;

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
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected override void ShootBulletClientRpc()
    {
        Instantiate(particleFire, _shootingPoint.position, _shootingPoint.rotation);
        
    }
}
