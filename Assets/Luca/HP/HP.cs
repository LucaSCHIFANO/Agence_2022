using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class HP : NetworkBehaviour
{
    [SerializeField] protected float maxHP;
    protected float currentHP;

    public virtual void Start()
    {
        currentHP = maxHP;
    }

    public void InitializeHP(float _hp)
    {
        if (currentHP != 0 || _hp <= 0f) return;

        currentHP = _hp;
    }

    public virtual void reduceHP(float damage)
    {
        currentHP -= damage;
        if(currentHP <= 0) Destroy(gameObject);
    }
}
