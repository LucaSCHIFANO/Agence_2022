using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class Sniper : Enemy
    {
        public override void Initialization(EnemySO _enemySo)
        {
            base.Initialization(_enemySo);
            
            target = GameObject.FindWithTag("Player");

            Vector3 nPos = (transform.position - target.transform.position).normalized * (range - 5) + target.transform.position;
            
            asker.AskNewPath(nPos, speed);
            targetLastPosition = target.transform.position;
        }
        
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            float distance = Vector3.Distance(transform.position, target.transform.position);
            
            if (distance <= range)
            {
                foreach (WeaponBase weapon in weapons)
                {
                    RaycastHit hit;

                    if (Physics.Raycast(weapon.transform.position, weapon.transform.forward, out hit))
                    {
                        if (hit.collider.CompareTag("Player"))
                        {
                            weapon.Shoot();
                        }
                    }
                }
            }

            if (Vector3.Distance(targetLastPosition, target.transform.position) > 5 && (distance <= range - 5 || distance > range))
            {
                Vector3 nPos = (transform.position - target.transform.position).normalized * (range - 5) + target.transform.position;
                
                asker.AskNewPath(nPos, speed);
                targetLastPosition = target.transform.position;
            }
        }
    }
}