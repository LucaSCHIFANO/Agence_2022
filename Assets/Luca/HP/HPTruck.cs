using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPTruck : HP
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
        
        if (currentHP <= 0)
        {
            App.Instance.Disconnect();
        }
        
    }
    
}
