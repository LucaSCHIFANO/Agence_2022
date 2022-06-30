using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class HPAntenna : HP
{
    public bool broken;
    [SerializeField] protected  GameObject thunderEffect;

    [SerializeField] protected Transform effectSpawn;
    [SerializeField] protected GameObject activateOnDeath;

    private bool isDeath;
    [SerializeField] protected GameObject explosionEffect;
    [SerializeField] protected Vector3 plusOuMoins;

    protected float timeActu;
    [SerializeField] private Vector2 timeBtwExplo;

    private void Update()
    {
        if (isDeath && timeActu <= 0)
        {
            Vector3 newPos = new Vector3(
                Random.Range(effectSpawn.transform.position.x + plusOuMoins.x, effectSpawn.transform.position.x - plusOuMoins.x),
                Random.Range(effectSpawn.transform.position.y + plusOuMoins.y, effectSpawn.transform.position.y - plusOuMoins.y),
                Random.Range(effectSpawn.transform.position.z + plusOuMoins.z, effectSpawn.transform.position.z - plusOuMoins.z));
            Instantiate(explosionEffect, newPos, effectSpawn.rotation);
            timeActu = Random.Range(timeBtwExplo.x, timeBtwExplo.y);
        }
        else timeActu -= Time.deltaTime;
        
    }

    public override void reduceHPToServ(float damage)
    {
        if(Runner.IsServer) TrueReduceHP(damage);
    }
    
    public override void TrueReduceHP(float damage)
    {
        if (isDeath) return;
        
        currentHP -= damage;
        SoundRPC();
        
        if (currentHP <= 0)
        {
            broken = true;
            Boss.Instance.checkAntenna();
            DestroyAntenna();
        }
    }
    
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void SoundRPC()
    {
        GetComponent<SoundTransmitter>()?.Play("Hit");
        Instantiate(thunderEffect, effectSpawn.position, effectSpawn.rotation);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void DestroyAntenna()
    {
        isDeath = true;
        activateOnDeath.SetActive(true);
    }
    
    
    
}
