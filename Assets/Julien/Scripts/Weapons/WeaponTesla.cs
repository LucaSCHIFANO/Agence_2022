using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WeaponTesla : WeaponBase
{
    [Header("Tesla Config")] [SerializeField]
    private ParticleSystem _particleSystem;

    public override void Shoot()
    {
        if (bulletLeft <= 0) return;
        
        base.Shoot();
        
        _particleSystem.Play();
    }
}
