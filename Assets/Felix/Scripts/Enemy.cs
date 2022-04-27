using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected bool isDead;
    protected int actualHealth;

    protected WeaponBase[] weapons; // WeaponSO type

    [SerializeField] protected Transform[] weaponsPosition;

    [SerializeField] protected Transform shootObject; // DEBUG
    
    public virtual void Initialization(EnemySO _enemySo)
    {
        actualHealth = _enemySo.health;

        // TEMP
        GameObject[] weaponsObject;
        
        if (_enemySo.weapons.Length >= weaponsPosition.Length)
        {
            weaponsObject = new GameObject[weaponsPosition.Length];

            for (int i = 0; i < weaponsPosition.Length; i++)
            {
                weaponsObject[i] = _enemySo.weapons[i];
            }
        }
        else
        {
            weaponsObject = new GameObject[_enemySo.weapons.Length];

            for (int i = 0; i < _enemySo.weapons.Length; i++)
            {
                weaponsObject[i] = _enemySo.weapons[i];
            }
        }

        if (weaponsObject != null)
        {
            weapons = new WeaponBase[weaponsObject.Length];
            
            for (int i = 0; i < weaponsObject.Length; i++)
            {
                GameObject nWeapon = Instantiate(weaponsObject[i], weaponsPosition[i].position, weaponsPosition[i].rotation);
                nWeapon.transform.parent = weaponsPosition[i];
                weapons[i] = nWeapon.GetComponent<WeaponBase>();
            }
        }
        // TEMP
        
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            //weapons[i].gameObject.transform.rotation // ROTATE WEAPON
            weapons[i].Shoot();
        }
    }

    public virtual void TakeDamage(int _damages)
    {
        if (isDead) return;
        
        if (_damages < 0)
            _damages = Mathf.Abs(_damages);

        actualHealth -= _damages;
        
        if (actualHealth <= 0)
            Die();
    }

    public virtual void TakeDamage(int _damages, Vector3 _collisionPoint, Vector3 _collisionDirection)
    {
        if (isDead) return;
        
        // Push, or work with vehicle physic
        // Spawn particles
        // Weak points ?

        TakeDamage(_damages);
    }

    protected virtual void Die()
    {
        isDead = true;
        
        // Destroy anim, explosion, ...
        // Drop loot
        
        // TEMP
        Destroy(gameObject);
    }
}
