using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected bool isDead;
    protected int actualHealth;

    protected GameObject[] weapons; // WeaponSO type

    [SerializeField] protected Transform[] weaponsPosition;
    
    public virtual void Initialization(EnemySO _enemySo)
    {
        actualHealth = _enemySo.health;

        // TEMP
        if (_enemySo.weapons.Length > 1)
        {
            weapons = new GameObject[1];

            int randomWeapon = Random.Range(0, _enemySo.weapons.Length);
            weapons[0] = _enemySo.weapons[randomWeapon];
        }
        else if (_enemySo.weapons.Length == 1)
        {
            weapons = new GameObject[1];
            weapons[0] = _enemySo.weapons[0];
        }

        if (weapons != null)
        {
            GameObject nWeapon = Instantiate(weapons[0], weaponsPosition[0].position, Quaternion.identity);
            nWeapon.transform.parent = weaponsPosition[0];
        }
        // TEMP
        
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
