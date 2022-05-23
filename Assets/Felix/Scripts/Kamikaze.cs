using System;
using System.Collections;
using System.Collections.Generic;
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

            asker.AskNewPath(target.transform, speed);
            targetLastPosition = target.transform.position;
        }

        protected override void FixedUpdate()
        {
            //base.FixedUpdate(); // No weapons so don't need to call it
            
            if (Physics.CheckBox(transform.position, transform.localScale + Vector3.one * range, transform.rotation,
                playersLayerMask))
            {
                Explode();
            }

            if (Vector3.Distance(targetLastPosition, target.transform.position) >= range)
            {
                asker.AskNewPath(target.transform, speed);
                targetLastPosition = target.transform.position;
            }
        }

        private void Explode()
        {
            Debug.Log("Boom");

            Die();
        }
    }
}
