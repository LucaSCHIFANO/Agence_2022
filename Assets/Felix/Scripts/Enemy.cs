using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected bool isDead;
    protected int actualHealth;

    [SerializeField] protected int maxHealth;

    public virtual void Initialization(EnemySO _enemySo)
    {
        actualHealth = _enemySo.health;
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
