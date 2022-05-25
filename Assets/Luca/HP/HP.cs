using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class HP : MonoBehaviour
{
    [SerializeField] protected float maxHP;
    protected float currentHP;

    private void Start()
    {
        currentHP = maxHP;
    }

    public virtual void reduceHP(float damage)
    {
        currentHP -= damage;
        if(currentHP <= 0) Destroy(gameObject);
    }
}
