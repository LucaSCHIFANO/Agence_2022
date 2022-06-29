using System;
using System.Collections;
using System.Collections.Generic;
using GameUI;
using UnityEngine;

public class HPPlayer : HP
{
    [SerializeField][Range(0, 1)] protected float hpPourcent;
    
    [SerializeField] protected float timeBeforeRecov;
    protected float currentTimeRecov;
    [SerializeField] protected float recovPerSecond;
    private bool wasDeadBefore;
    
    private void Update()
    {
        HP();
    }

    void HP()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(Object.HasInputAuthority) reduceHPToServ(10f);
        }

        if (currentTimeRecov <= 0) currentHP += Time.deltaTime * recovPerSecond;
        else currentTimeRecov -= Time.deltaTime;
 
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        hpPourcent = currentHP / maxHP;
        
        if(Object.HasInputAuthority) CanvasInGame.Instance.actuBlood(Mathf.Abs(hpPourcent - 1));
        if (currentHP <= 0 && !wasDeadBefore)
        {
            // Dead
            wasDeadBefore = true;
            GameOverManager.instance.PlayerDied();
        }

    IEnumerator RespawnPlayer()
    {
        if (Object.HasStateAuthority)
        {
            yield return new WaitForSecondsRealtime(respawnDelay);
            wasDeadBefore = false;
            currentHP = maxHP;
            TruckPhysics truckPhysics = _truckReference.Acquire();
            Transform respawnPoint = truckPhysics.RespawnPoints[Random.Range(0, truckPhysics.RespawnPoints.Count)];
            ccCustom.TeleportToPositionRotation(respawnPoint.position, respawnPoint.rotation);
            HideDeathScreen();
            GameOverManager.instance.PlayerRespawn();
        }
    }
    
    public override void TrueReduceHP(float damage)
    {
        currentHP -= damage;
        currentTimeRecov = timeBeforeRecov;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    private void ShowDeathScreen()
    {
        _deathScreen.SetActive(true);
        _player.ChangeInputHandler(PossessingType.NONE, gameObject);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    private void HideDeathScreen()
    {
        _deathScreen.SetActive(false);
        _player.ChangeInputHandler(PossessingType.CHARACTER, gameObject);
    }
}
