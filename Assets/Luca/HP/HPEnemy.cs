using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPEnemy : HP
{
    public GameObject impactEffect;
    
    public override void TrueReduceHP(float damage)
    {
        currentHP -= damage;
        Instantiate(impactEffect, transform.position, transform.rotation);
        
        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
