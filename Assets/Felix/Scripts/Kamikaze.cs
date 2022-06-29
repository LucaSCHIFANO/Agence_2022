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

        public override void Initialization(EnemySO _enemySo)
        {
            base.Initialization(_enemySo);

            target = GameObject.FindWithTag("Player");

            if (Runner.IsServer && target != null)
            {
                asker.AskNewPath(target.transform, speed, null);
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
            
            if (!Runner.IsServer || asker == null)
                return;
            
            if (Physics.CheckBox(transform.position, transform.localScale + Vector3.one * range, transform.rotation,
                playersLayerMask))
            {
                Explode();
            }

            if (Vector3.Distance(targetLastPosition, target.transform.position) >= range)
            {
                asker.AskNewPath(target.transform, speed, null, false);
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
