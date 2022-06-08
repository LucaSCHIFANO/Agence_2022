using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPSubTruck : HP
{
    [SerializeField] private HPTruck hptruck; 
    public override void reduceHP(float damage)
    {
        hptruck.reduceHP(damage);
    }
}
