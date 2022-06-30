using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class BulletDamage : NetworkBehaviour
{
    public float damageValue;
    
    
    private void OnCollisionEnter(Collision other)
    {
        
        if (other.gameObject.TryGetComponent(out HP hp)) hp.reduceHPToServ(damageValue);
            
        Destroy(gameObject);
        
    }
}
