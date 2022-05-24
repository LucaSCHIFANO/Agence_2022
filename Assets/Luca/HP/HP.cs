using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class HP : MonoBehaviour
{
    [SerializeField] protected float maxHP;
    private float currentHP;

    private void Start()
    {
        currentHP = maxHP;
    }

    public void reduceHP(float damage)
    {
        currentHP -= damage;
        Debug.Log(currentHP);
        if(currentHP <= 0) Destroy(gameObject);
    }
}
