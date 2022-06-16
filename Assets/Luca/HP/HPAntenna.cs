using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class HPAntenna : HP
{
    public bool broken;
    public GameObject thunderEffect;
    
    public override void reduceHPToServ(float damage)
    {
        if(Runner.IsServer) TrueReduceHP(damage);
    }
    
    public override void TrueReduceHP(float damage)
    {
        currentHP -= damage;
        SoundRPC();
        
        if (currentHP <= 0)
        {
            broken = true;
            Boss.Instance.checkAntenna();
            
        }
    }
    
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void SoundRPC()
    {
        GetComponent<SoundTransmitter>()?.Play("Hit");
        Instantiate(thunderEffect, transform.position, transform.rotation);
    }
    
}
