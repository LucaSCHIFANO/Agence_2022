using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTesla : WeaponBase
{
    [Header("Tesla Config")] [SerializeField]
    private ParticleSystem _particleSystem;

    public override void Shoot()
    {
        _particleSystem.Play();
        
        base.Shoot();
    }
}
