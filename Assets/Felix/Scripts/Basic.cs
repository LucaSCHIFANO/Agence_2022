using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class Basic : Enemy
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
                        if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Car"))
                        {
                            weapon.Shoot();
                        }
                    }
                }
            }

            if (Vector3.Distance(targetLastPosition, target.transform.position) >= range)
            {
                asker.AskNewPath(FindNewPosition(), speed, null, false);
                targetLastPosition = target.transform.position;
            }
        }

        private Vector3 FindNewPosition()
        {
            float distance = range / 10;
            
            for (int i = -1; i <= 1; i += 2)
            {
                Vector3 dir = target.transform.right * i;
                if (Physics.Raycast(target.transform.position, dir, out RaycastHit hit, distance, obstaclesLayerMask))
                {
                    if (Vector3.Distance(target.transform.position, hit.point) > range)
                    {
                        return target.transform.position + target.transform.right * i * (distance - distance / 2);
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

            return target.transform.position + target.transform.forward * (isBehind ? -1f : 1f) * distance / 2;
        }

        private bool isPointLeft(Vector2 _a, Vector2 _b, Vector2 _point)
        {
            return ((_b.x - _a.x)*(_point.y - _a.y) - (_b.y - _a.y)*(_point.x - _a.x)) > 0;
        }
    }
}