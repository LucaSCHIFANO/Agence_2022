using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponUltima : WeaponBase
{

    [SerializeField] protected weapon actualWeapon;
    [SerializeField] protected WeaponFireType fireType;

    [SerializeField] protected float maxDistance;

    [SerializeField] protected float spread;
    [SerializeField] protected int numberOfShot;
    [SerializeField] protected float damage;
    
    [SerializeField] protected GameObject particleFire;
    [SerializeField] protected WScriptable startingWeapon;
    
    
    public enum weapon
    {
        BASIC,
        BURST,
        MACHINEGUN,
        FLAMETHROWER,
        SHOTGUN,
        SNIPER,
        TESLA,
    }

    private int shootedRound;
    public bool isShooting;

    private void Start()
    {
        actuAllStats(startingWeapon);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        
        if (actualWeapon == weapon.BASIC || actualWeapon == weapon.BURST)
        {
            if (isShooting && isPossessed)
            {
                Shoot();
            }
        }
    }


    public void actuAllStats(WScriptable SObject)
    {
        if (Object == null) return;

        GetComponent<WeaponInteractable>().weaponName = SObject.turretName;
        _fireRate = SObject.fireRate;
        _bulletToOverHeat = SObject.bulletToOverheat;
        _coolDownPerSecond = SObject.coolDownPerSecond;
        _timeBeforeCoolDown = SObject.timeBeforeCoolDown;
        maincolor = SObject.mainColor;
        overHeatColor = SObject.overHeatColor;
        actualWeapon = SObject.wType;
        fireType = SObject.fType;
        maxDistance = SObject.maxDistanceRayCast;
        shootParticle = SObject.shootingEffect;
        
        
        spread = SObject.spread;
        numberOfShot = SObject.numberOfShots;
        damage = SObject.damage;

        _isOverHeat = false;
        _isCoolDown = false;
        isShooting = false;
        overHeatPourcentOnline = 0;
        _shootingTimer = 0;
        _timeCoolDown = 0;
    }

    public override void Shoot()
    {
        if (Object == null) return;
        if (!Runner.IsServer) return;
        if (actualWeapon == weapon.BASIC || actualWeapon == weapon.BURST) isShooting = true;
        
        if (_shootingTimer > 0) return;
        if (_isOverHeat) return;
        
        base.Shoot();
        
        
        if (fireType == WeaponFireType.Hitscan)
        {
            ShootEffectClientRpc();

            if (actualWeapon == weapon.SHOTGUN)
            {
                for (int i = 0; i < numberOfShot; i++)
                {
                    RaycastHit hit;
                    Vector3 shootingDir = Quaternion.Euler(Random.Range(-spread, spread), Random.Range(-spread, spread),
                        Random.Range(-spread, spread)) * _shootingPoint.forward;
                    
                    if (Physics.Raycast(_shootingPoint.position, shootingDir, out hit, maxDistance))
                    {

                        // Instantiate(bulletEffect, , transform.rotation);

                        if (hit.collider.gameObject.TryGetComponent(out HP hp))
                        {
                            if (allieTouret)
                            {
                                if (hp is HPPlayer || hp is HPSubTruck || hp is HPTruck) return;
                                hp.reduceHPToServ(damage * (Generator.Instance.pourcentageList[0] / 100));
                            }
                            else
                            {
                                if ((hp is HPPlayer || hp is HPTruck)) hp.reduceHPToServ(damage);
                            }


                        }
                        else
                        {
                            BulletEffectClientRpc(hit.point);
                        }
                    }
                }
            }
            else
            {
                RaycastHit hit;
                Vector3 shootingDir = Quaternion.Euler(Random.Range(-spread, spread), Random.Range(-spread, spread),
                    Random.Range(-spread, spread)) * _shootingPoint.forward;
                Debug.DrawRay(_shootingPoint.position, shootingDir * 1000, Color.red, 10);
                // LagCompensatedHit hit;
                // Runner.LagCompensation.Raycast(_shootingPoint.position, shootingDir, 100, Object.InputAuthority, out hitComp) <- Only check if enemy are hit
                if (Physics.Raycast(_shootingPoint.position, shootingDir, out hit, maxDistance))
                {

                    // Instantiate(bulletEffect, , transform.rotation);

                    if (hit.collider.gameObject.TryGetComponent(out HP hp))
                    {
                        if (allieTouret)
                        {
                            if (hp is HPPlayer || hp is HPSubTruck || hp is HPTruck) return;
                            hp.reduceHPToServ(damage * (Generator.Instance.pourcentageList[0] / 100));
                        }
                        else
                        {
                            if ((hp is HPPlayer || hp is HPTruck)) hp.reduceHPToServ(damage);
                        }


                    }
                    else
                    {
                        BulletEffectClientRpc(hit.point);
                    }
                }
            }

        }
        else
        {
            ShootBulletServerRpc();
            shootWeaponBullet();
        }

        if (actualWeapon == weapon.BASIC || actualWeapon == weapon.BURST)
        {

            shootedRound++;
            _shootingTimer = .1f;

            if (shootedRound >= numberOfShot)
            {
                shootedRound = 0;
                _shootingTimer = 1 / _fireRate;
                isShooting = false;
            }
        }else _shootingTimer = 1 / _fireRate;
    }
    
    
    
    
    
    
    
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected override void ShootBulletClientRpc()
    {
        // if(IsOwner) return;
        
        if(fireType == WeaponFireType.Projectile) shootWeaponBullet();
        
        
    }


    void shootWeaponBullet()
    {
        switch (actualWeapon)
        {
            case weapon.BASIC: case weapon.BURST : case weapon.MACHINEGUN :
                Instantiate(_bulletPrefab, _shootingPoint.position, _shootingPoint.rotation * Quaternion.Euler(new Vector3(Random.Range(-spread, spread),
                    Random.Range(-spread, spread), Random.Range(-spread, spread))));
                break;
            
            case weapon.FLAMETHROWER :
                Instantiate(particleFire, _shootingPoint.position, _shootingPoint.rotation);
                break;
            
            case weapon.SHOTGUN :
                for (int i = 0; i < numberOfShot; i++)
                {
                    Instantiate(_bulletPrefab, _shootingPoint.position,
                        _shootingPoint.rotation * Quaternion.Euler(new Vector3(Random.Range(-spread, spread),
                            Random.Range(-spread, spread), Random.Range(-spread, spread))));
                }
                break;
            
            case weapon.SNIPER : case weapon.TESLA :
                Instantiate(_bulletPrefab, _shootingPoint.position, _shootingPoint.rotation);
                break;

        }
    }
    
}