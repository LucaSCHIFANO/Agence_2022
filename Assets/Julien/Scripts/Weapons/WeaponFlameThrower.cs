using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WeaponFlameThrower : WeaponBase
{
    [Header("Flamethrower Config")] [SerializeField]
    private ParticleSystem _particleSystem;

    public override void Shoot()
    {
        if (_shootingTimer > 0) return;
        if (_isOverHeat) return;
        base.Shoot();

        _particleSystem.Play();
    }
}
