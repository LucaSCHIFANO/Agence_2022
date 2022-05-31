using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Enemies
{
    public class Enemy : NetworkBehaviour
    {
        protected HPEnemy hp;
        protected Asker asker;

        protected GameObject target;
        
        protected Vector3 targetLastPosition;

        protected float speed;
        public float range;
        protected bool isDead;
        protected int actualHealth;

        protected WeaponBase[] weapons; // WeaponSO type
        [SerializeField] protected EnemySO enemySo; // TEMP

        [SerializeField] protected Transform[] weaponsPosition;

        protected void Awake()
        {
            hp = GetComponent<HPEnemy>();
            
            if (asker == null)
            {
                Initialization(enemySo);
            }
        }

        public virtual void Initialization(EnemySO _enemySo)
        {
            asker = GetComponent<Asker>();

            actualHealth = _enemySo.health;
            speed = _enemySo.speed;
            range = _enemySo.range;

            if (_enemySo.weapons.Length != 0)
            {
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

                weapons = new WeaponBase[weaponsObject.Length];

                for (int i = 0; i < weaponsObject.Length; i++)
                {
                    GameObject nWeapon = Instantiate(weaponsObject[i], weaponsPosition[i]);
                    weapons[i] = nWeapon.GetComponent<WeaponBase>();
                } 
            }
        }

        protected virtual void FixedUpdate()
        {
            foreach (WeaponBase weapon in weapons)
            {
                weapon.transform.LookAt(target.transform);
            }
        }

        public virtual void TakeDamage(int _damages)
        {
            if (isDead) return;

            if (_damages < 0)
                _damages = Mathf.Abs(_damages);

            hp.reduceHP(_damages);
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
}