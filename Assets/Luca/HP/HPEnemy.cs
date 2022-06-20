using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class HPEnemy : HP
{
    public GameObject impactEffect;
    
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
            Destroy(gameObject);
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SoundRPC()
    {
        GetComponent<SoundTransmitter>()?.Play("Hit");
        Instantiate(impactEffect, transform.position, transform.rotation);
    }
    
       

}
