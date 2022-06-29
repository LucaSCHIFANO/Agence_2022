using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class HPTruck : HP
{
    [SerializeField] private GameObject impactEffect;
    private TruckPhysics truck;
    private UpgradeMenu menu;
    private float reductionAt100;
    [SerializeField] protected WinRef winReference;
    
    public float currenthealth { get { return currentHP; } }
    public float maxhealth { get { return maxHP; } }

    private void Start()
    {
        truck = GetComponent<TruckPhysics>();
        truck.activateDamageParticle(100f);
    }

    public override void reduceHPToServ(float damage)
    {
        if(Runner.IsServer) TrueReduceHP(damage);
    }
    
    public override void TrueReduceHP(float damage)
    {
        var reduction = 0f;
        if (UpgradeMenu.Instance.upgradesC[1] != 0) reduction = (Generator.Instance.pourcentageList[1] * reductionAt100) / 100;

        currentHP -= (damage * (1 - (reduction) / 100));
        SoundRPC();
        particleVisuRpc();
        

        if (currentHP <= 0)
        {
            if (Runner)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                RPC_EndGame();
            }
        }

    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_EndGame()
    {
        WinManager _winReference = winReference.Acquire();
        _winReference.callTheGameOver();
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void particleVisuRpc()
    {
        var pourcent = (currentHP / maxHP) * 100;
        truck.activateDamageParticle(pourcent);
        
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void SoundRPC()
    {
        GetComponent<SoundTransmitter>()?.Play("Hit");
        Instantiate(impactEffect, transform.position, transform.rotation);
    }

    public void heal(bool fullHeal)
    {
        if (fullHeal) currentHP = maxHP;
        else currentHP += (maxHP / 10);

        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
    }
    
}
