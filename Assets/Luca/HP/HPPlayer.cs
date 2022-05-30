using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPPlayer : HP
{
    [SerializeField][Range(0, 1)] protected float hpPourcent;
    
    [SerializeField] protected float timeBeforeRecov;
    protected float currentTimeRecov;
    [SerializeField] protected float recovPerSecond;

    private void Update()
    {
        HP();
    }

    void HP()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            reduceHP(10f);
        }

        if (currentTimeRecov <= 0) currentHP += Time.deltaTime * recovPerSecond;
        else currentTimeRecov -= Time.deltaTime;
 
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        hpPourcent = currentHP / maxHP;
        
        CanvasInGame.Instance.actuBlood(Mathf.Abs(hpPourcent - 1));

    }
    
    public override void reduceHP(float damage)
    {
        currentHP -= damage;
        currentTimeRecov = timeBeforeRecov;
    }
}
