using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameOverManager : SimulationBehaviour
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
        if (Runner.IsServer)
        {
            if (playerDead == App.Instance.Players.Count)
            {
                App.Instance.Session.LoadMap(MapIndex.GameOver);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
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
}
