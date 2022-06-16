using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPTruck : HP
{
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private float pourcentageCritique;
    private TruckPhysics truck;

    private void Start()
    {
        truck = GetComponent<TruckPhysics>();
        truck?.activateFire(false);
    }

    public override void reduceHPToServ(float damage)
    {
        if(Runner.IsServer) TrueReduceHP(damage);
    }
    
    public override void TrueReduceHP(float damage)
    {
        currentHP -= damage;
        Instantiate(impactEffect, transform.position, transform.rotation);

        if (currentHP <= pourcentageCritique) truck?.activateFire(true);
        else truck?.activateFire(false);

        if (currentHP <= 0)
        {
            if (Runner)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                App.Instance.Session.LoadMap(MapIndex.GameOver);
            }
        }

    }
    
}
