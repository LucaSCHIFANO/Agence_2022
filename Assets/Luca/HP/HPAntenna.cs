using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class HPAntenna : HP
{
    public bool broken;
    public GameObject thunderEffect;


    public override void Start()
    {
        currentHP = maxHP;
    }

    public override void TrueReduceHP(float damage)
    {
        currentHP -= damage;
        Instantiate(thunderEffect, transform.position, transform.rotation);
        
        if (currentHP <= 0)
        {
            broken = true;
            Boss.Instance.checkAntenna();
            
        }
    }
    
}
