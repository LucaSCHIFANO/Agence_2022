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
            
            asker.AskNewPath(nPos, speed, OnPathFound);
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
                    if (Physics.Raycast(weapon.transform.position, weapon.transform.forward, out RaycastHit hit))
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
                
                asker.AskNewPath(nPos, speed, OnPathFound);
                targetLastPosition = target.transform.position;
            }
        }

        public void OnPathFound(Vector3[] _points)
        {
            if (_points.Length == 0) return;
            
            Vector3 direction = target.transform.position - _points[^1];

            if (Physics.Raycast(_points[^1], direction.normalized, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    print("Sniper will see player at end");
                    return;
                }
            }
            
            direction.y = 0;
            direction.Normalize();
            
            print("Sniper will not see player at end,looking for a new  position");

            for (int i = 5; i < 360; i+=5)
            {
                float x2 = Mathf.Cos(i * direction.x) - Mathf.Sin(i*direction.z);
                float z2 = Mathf.Sign(i * direction.x) + Mathf.Cos(i * direction.z);

                Vector3 nVector = new Vector3(x2, 0, z2) * range;
                nVector.y = target.transform.position.y;
                
                if (Physics.Raycast(nVector, (target.transform.position - nVector).normalized, out RaycastHit hit2))
                {
                    if (hit2.collider.CompareTag("Player"))
                    {
                        print("Sniper found a new position where he will see player at end");
                        asker.AskNewPath(nVector, speed, null);
                        return;
                    }
                }
            }
        }
    }
}