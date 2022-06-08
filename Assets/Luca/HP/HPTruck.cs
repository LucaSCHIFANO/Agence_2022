using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPTruck : HP
{
    public GameObject impactEffect;
    
    public override void reduceHP(float damage)
    {
        currentHP -= damage;
        Instantiate(impactEffect, transform.position, transform.rotation);
        if (currentHP <= 0)
        {
            App.Instance.Disconnect();
        }
    }
}
