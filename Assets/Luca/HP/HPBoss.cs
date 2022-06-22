using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class HPBoss : HP
{
    public GameObject impactEffect;
    [SerializeField] protected ParticleSystem dust;
    [SerializeField] protected WinRef winReference;
    
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
            RPC_ShowCursor();
            dust.Stop();
            WinManager _winReference = winReference.Acquire();
            _winReference.callTheEnd();
            //App.Instance.Session.LoadMap(MapIndex.Win);
        }
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SoundRPC()
    {
        GetComponent<SoundTransmitter>()?.Play("Hit");
        Instantiate(impactEffect, transform.position, transform.rotation);
    }
}
