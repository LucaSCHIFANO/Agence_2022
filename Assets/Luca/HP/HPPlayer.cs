using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using GameUI;
using UnityEngine;
using Random = UnityEngine.Random;

public class HPPlayer : HP
{
    [SerializeField][Range(0, 1)] protected float hpPourcent;
    
    [SerializeField] protected float timeBeforeRecov;
    protected float currentTimeRecov;
    [SerializeField] protected float recovPerSecond;
    private bool wasDeadBefore;

    [SerializeField] private TruckReference _truckReference;
    [SerializeField] private GameObject _deathScreen;
    [SerializeField] private float respawnDelay;

    private NetworkedPlayer _player;
    private NetworkCharacterControllerPrototypeCustom ccCustom;
    
    public override void Spawned()
    {
        base.Spawned();
        _player = GetComponent<NetworkedPlayer>();
        ccCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
    }

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

        if(Object.HasInputAuthority && !wasDeadBefore) CanvasInGame.Instance.actuBlood(Mathf.Abs(hpPourcent - 1));
        if (currentHP <= 0 && !wasDeadBefore)
        {
            // Dead
            wasDeadBefore = true;
            CanvasInGame.Instance.actuBlood(0);
            ShowDeathScreen();
            GameOverManager.instance.PlayerDied();
            StartCoroutine(RespawnPlayer());
        }
    }

    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSecondsRealtime(respawnDelay);
        wasDeadBefore = false;
        currentHP = maxHP;
        HideDeathScreen();
        GameOverManager.instance.PlayerRespawn();
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
        TruckPhysics truckPhysics = _truckReference.Acquire();
        Transform respawnPoint = truckPhysics.RespawnPoints[Random.Range(0, truckPhysics.RespawnPoints.Count)];
        ccCustom.TeleportToPositionRotation(respawnPoint.position, respawnPoint.rotation);
        _deathScreen.SetActive(false);
        _player.ChangeInputHandler(PossessingType.CHARACTER, gameObject);
    }
}
