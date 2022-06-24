using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class Sniper : Enemy
    {
        [SerializeField] private LayerMask obstaclesLayerMask;

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (!Runner.IsServer || asker == null || !isChasing)
                return;
            
            float distance = Vector3.Distance(transform.position, target.transform.position);
        
            if (distance <= range)
            {
                foreach (WeaponUltima weapon in weapons)
                {
                    if (Physics.Raycast(weapon.transform.position, weapon.transform.forward, out RaycastHit hit))
                    {
                        weapon.Shoot();
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
            
            Vector3 direction = target.transform.position + new Vector3(0f, 1f, 0f) - _points[^1];

            if (Physics.Raycast(_points[^1], direction.normalized, out RaycastHit hit, range, obstaclesLayerMask))
            {
                Transform highParent = hit.transform;
                while (highParent.parent != null)
                {
                    highParent = highParent.parent;
                }
                
                if (highParent.CompareTag("Player") || highParent.CompareTag("Car"))
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
                float x2 = Mathf.Cos(i * direction.x) - Mathf.Sin(i * direction.z);
                float z2 = Mathf.Sign(i * direction.x) + Mathf.Cos(i * direction.z);

                Vector3 nVector = new Vector3(x2, 0, z2) * range;
                nVector.y = target.transform.position.y;

                if (Physics.Raycast(nVector, (target.transform.position + new Vector3(0f, 1f, 0f) - nVector).normalized, out hit, range, obstaclesLayerMask))
                {
                    Transform highParent = hit.transform;
                    while (highParent.parent != null)
                    {
                        highParent = highParent.parent;
                    }
                    
                    if (highParent.CompareTag("Player") || highParent.CompareTag("Car"))
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