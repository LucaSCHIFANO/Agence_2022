using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPAntenna : HP
{
    [HideInInspector] public bool broken;
    
    public override void reduceHP(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            broken = true;
            Boss.Instance.checkAntenna();
        }
    }
}
