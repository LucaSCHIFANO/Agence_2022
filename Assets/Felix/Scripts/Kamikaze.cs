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

            if (Runner.IsServer)
            {
                asker.AskNewPath(target.transform, speed, null);
                targetLastPosition = target.transform.position;
            }
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            
            if (!Runner.IsServer)
                return;
            
            if (Physics.CheckBox(transform.position, transform.localScale + Vector3.one * range, transform.rotation,
                playersLayerMask))
            {
                Explode();
            }

            if (Vector3.Distance(targetLastPosition, target.transform.position) >= range * speed / 2)
            {
                asker.AskNewPath(target.transform, speed, null);
                targetLastPosition = target.transform.position;
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void Explode()
        {
            Debug.Log("Boom");

            Die();
        }
    }
}
