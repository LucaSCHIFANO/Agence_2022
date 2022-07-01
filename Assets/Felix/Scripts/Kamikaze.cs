using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Enemies
{
    public class Kamikaze : Enemy
    {
        [SerializeField] private LayerMask playersLayerMask;

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            
            if (!Runner.IsServer || asker == null || !isChasing)
                return;
            
            if (Physics.CheckBox(transform.position, transform.localScale + Vector3.one * range, transform.rotation,
                playersLayerMask))
            {
                Explode();
            }

            if (Vector3.Distance(targetLastPosition, target.transform.position) >= range)
            {
                asker.AskNewPath(target.transform, speed, null, true);
                /*if (asker.pathEnd)
                {
                    asker.AskNewPath(target.transform, speed, null);
                }
                else
                {
                    // Add line calcul
                    asker.AskNewPathAtEnd(target.transform.position);
                }*/

                targetLastPosition = target.transform.position;
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void Explode()
        {
            Debug.Log("Boom");

            Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);
            foreach (Collider collider in colliders)
            {
                HP hp = collider.GetComponent<HP>();
                if (hp != null)
                {
                    hp.TrueReduceHP(50f);
                }
            }

            Die();
        }
    }
}
