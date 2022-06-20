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

        protected bool isChasing;

        protected float speed;
        protected float range;
        protected bool isDead;

        protected WeaponUltima[] weapons;
        [SerializeField] protected EnemySO enemySo;

        [SerializeField] protected Transform[] weaponsPosition;

        // For waves
        protected NewNwWaves waves;

        protected void Awake()
        {
            hp = GetComponent<HPEnemy>();
        }

        public override void Spawned()
        {
            base.Spawned();
            
            if (asker == null)
            {
                Initialization(enemySo);
            }
        }

        public virtual void Initialization(EnemySO _enemySo)
        {
            asker = GetComponent<Asker>();
            hp = GetComponent<HPEnemy>();

            hp.InitializeHP(_enemySo.health);
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

                weapons = new WeaponUltima[weaponsObject.Length];

                for (int i = 0; i < weaponsObject.Length; i++)
                {
                    //GameObject nWeapon = Instantiate(weaponsObject[i], weaponsPosition[i]);
                    GameObject nWeapon = Runner.Spawn(weaponsObject[i].GetComponent<NetworkObject>(), weaponsPosition[i].position, weaponsPosition[i].rotation).gameObject;
                    nWeapon.transform.SetParent(weaponsPosition[i]);
                    WeaponUltima weaponUltima = nWeapon.GetComponent<WeaponUltima>();
                    weaponUltima.actuAllStats(enemySo.weaponsScriptable[i]);
                    weaponUltima.isPossessed = false;
                    
                    weapons[i] = weaponUltima;
                } 
            }
        }

        public void Chase(GameObject _target)
        {
            isChasing = true;
            target = _target;
        }

        public void StopChasing(Vector3 _returnPosition)
        {
            isChasing = false;
            asker.AskNewPath(_returnPosition, speed, null);
        }

        protected virtual void FixedUpdate()
        {
            if (target == null || weapons == null)
                return;
            
            foreach (WeaponUltima weapon in weapons)
            {
                weapon.transform.LookAt(target.transform);
            }
        }

        public virtual void TakeDamage(int _damages)
        {
            if (isDead) return;

            if (_damages < 0)
                _damages = Mathf.Abs(_damages);

            if(Object.HasInputAuthority) hp.reduceHPToServ(_damages);
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

        private void OnDestroy()
        {
            waves?.removeEnemy(this);
        }

        public void SetWaves(NewNwWaves _wave)
        {
            waves = _wave;
        }
    }
}