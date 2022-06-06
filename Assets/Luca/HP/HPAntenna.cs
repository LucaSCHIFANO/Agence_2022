using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class HPAntenna : HP
{
    public bool broken;
    public GameObject thunderEffect;
    [Networked(OnChanged = nameof(SendHPChanged))] private float HPOnline { get; set; }


    public override void Start()
    {
        currentHP = maxHP;
    }

    public override void reduceHP(float damage)
    {
        currentHP -= damage;
        HPOnline = currentHP;

        if (currentHP <= 0)
        {
            broken = true;
            Boss.Instance.checkAntenna();
            
        }
    }


    public static void SendHPChanged(Changed<HPAntenna> changed)
    {
        changed.Behaviour.SendHP();
    }

    private void SendHP()
    {
        Instantiate(thunderEffect, transform.position, transform.rotation);
        currentHP = HPOnline;
    }
}
