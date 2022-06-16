using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class HPEnemy : HP
{
    public GameObject impactEffect;
    
    public override void reduceHPToServ(float damage)
    {
        if(Runner.IsServer) TrueReduceHP(damage);
    }
    
    public override void TrueReduceHP(float damage)
    {
        currentHP -= damage;
        Instantiate(impactEffect, transform.position, transform.rotation);
        GetComponent<SoundTransmitter>()?.Play("Hit");
        
        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }
    
       

}
