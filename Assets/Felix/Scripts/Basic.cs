using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class Basic : Enemy
    {
        [SerializeField] private LayerMask obstaclesLayerMask;
        
        public override void Initialization(EnemySO _enemySo)
        {
            base.Initialization(_enemySo);
            
            target = GameObject.FindWithTag("Player");

            if (Runner.IsServer && target != null)
            {
                asker.AskNewPath(FindNewPosition(), speed, null);
                targetLastPosition = target.transform.position;
            }
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (target == null)
            {
                target = GameObject.FindWithTag("Player");
                
                return;
            }
            
            if (!Runner.IsServer)
                return;
            
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

            if (Vector3.Distance(targetLastPosition, target.transform.position) >= 2f)
            {
                asker.AskNewPath(FindNewPosition(), speed, null);
                targetLastPosition = target.transform.position;
            }
        }

        private Vector3 FindNewPosition()
        {
            float distance = 5f;
            
            for (int i = -1; i <= 1; i += 2)
            {
                Vector3 dir = target.transform.right * i;
                if (Physics.Raycast(target.transform.position, dir, out RaycastHit hit, distance, obstaclesLayerMask))
                {
                    if (Vector3.Distance(target.transform.position, hit.point) > 3.5f)
                    {
                        return target.transform.position + target.transform.right * i * (distance - 1.5f);
                    }
                }
                else
                {
                    return target.transform.position + target.transform.right * i * distance;
                }
            }

            bool isBehind = isPointLeft(
                new Vector2(-target.transform.right.x, -target.transform.right.z),
                new Vector2(target.transform.right.x, target.transform.right.z),
                new Vector2(transform.position.x, transform.position.z));

            return target.transform.position + target.transform.forward * (isBehind ? -1f : 1f) * 5f;
        }

        private bool isPointLeft(Vector2 _a, Vector2 _b, Vector2 _point)
        {
            return ((_b.x - _a.x)*(_point.y - _a.y) - (_b.y - _a.y)*(_point.x - _a.x)) > 0;
        }
    }
}