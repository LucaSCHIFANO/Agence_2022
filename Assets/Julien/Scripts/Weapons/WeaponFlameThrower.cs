using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFlameThrower : WeaponBase
{
    [Header("Flamethrower Config")] [SerializeField]
    private ParticleSystem _particleSystem;

    public override void Shoot()
    {
        _particleSystem.Play();
        
        base.Shoot();
    }
}
