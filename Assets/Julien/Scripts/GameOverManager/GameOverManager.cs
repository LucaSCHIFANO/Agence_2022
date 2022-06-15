using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameOverManager : NetworkBehaviour
{
    public static GameOverManager instance;

    private int playerDead;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        instance = this;
    }

    void Update()
    {
        if (Runner)
        {
            if (Runner.IsServer)
            {
                Debug.Log($"{playerDead} player dead");
                if (playerDead == App.Instance.Players.Count)
                {
                    RPC_ShowCursor();
                    App.Instance.Session.LoadMap(MapIndex.GameOver);
                }
            }
        }
    }

    public void PlayerDied()
    {
        playerDead++;
    }

    public void PlayerRespawn()
    {
        playerDead--;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
