using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.Jobs;
using UnityEngine;

public class Shop : NetworkBehaviour
{
    private NetworkedPlayer _playerController;
    [Networked] private bool isPossessed { get; set; }
    public TruckArea truckArea;

    private PlayerRef _playerRef;

    #region Singleton

    private static Shop instance;

    public static Shop Instance
    {
        get => instance;
        set => instance = value;
    }

    #endregion

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        
        Instance = this;
    }


    public void quitShop()
    {
        _playerController.ChangeInputHandler(PossessingType.CHARACTER, gameObject);
        UpgradeMenu.Instance.gotoScreen(0);
        CanvasInGame.Instance.showShop(false);

        // ReactivateController(_playerRef);
        if (_playerController.Object.HasInputAuthority)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _playerController.CharacterInputHandler.enabled = true;
            isPossessed = false;
        }
    }

    public void Interact(NetworkedPlayer other)
    {
        if (isPossessed) return;

        var okay = false;
        foreach (var VARIABLE in truckArea.objectInAreaTruck)
        {
            if (VARIABLE.gameObject.tag == "Car")
            {
                okay = true;
                break;
            }
        }

        if (!okay) return;

        if (other.Object.HasInputAuthority)
        {
            other.ChangeInputHandler(PossessingType.NONE, gameObject);
            CanvasInGame.Instance.showShop(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _playerController = other;
            SetPlayerController();
            
            isPossessed = true;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void SetPlayerController(RpcInfo rpcInfo = default)
    {
        _playerRef = rpcInfo.Source;
    }

    /*[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void ReactivateController([RpcTarget] PlayerRef playerRef)
    {
        Runner.GetPlayerObject(playerRef).GetComponent<NetworkedPlayer>()
            .ChangeInputHandler(PossessingType.CHARACTER, gameObject);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _playerController.CharacterInputHandler.enabled = true;
        isPossessed = false;
    }*/
}