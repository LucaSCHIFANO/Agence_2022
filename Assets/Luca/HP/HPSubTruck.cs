using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPSubTruck : HP
{
    [SerializeField] private HPTruck hptruck; 
    
    public override void reduceHPToServ(float damage)
    {
        if(Runner.IsServer) TrueReduceHP(damage);
    }
}
