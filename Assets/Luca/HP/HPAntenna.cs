using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPAntenna : HP
{
    [HideInInspector] public bool broken;
    public GameObject thunderEffect;
    
    public override void reduceHP(float damage)
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
