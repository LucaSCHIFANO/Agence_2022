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
        
        private void FixedUpdate()
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            
            if (distance <= range)
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position, (transform.position - target.transform.position).normalized, out hit))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        Shoot(hit);
                    }
                }
            }

            if (distance <= range - 5 || distance > range)
            {
                Vector3 nPos = (transform.position - target.transform.position).normalized * (range - 5) + target.transform.position;
                
                asker.AskNewPath(nPos, speed);
                targetLastPosition = target.transform.position;
            }
        }

        private void Shoot(RaycastHit _hit)
        {
            // weapon shoot
            Debug.Log("Sniper shoot", _hit.collider.gameObject);
        }
    }
}