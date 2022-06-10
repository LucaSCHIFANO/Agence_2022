using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    public float damageValue;
    
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<HP>()) other.gameObject.GetComponent<HP>().reduceHPToServ(damageValue);
            
        Destroy(gameObject);
        
    }
}
