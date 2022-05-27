using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Netcode;
using UnityEngine;

public class Shop : NetworkBehaviour
{
    private PlayerController _playerController;
    private NetworkVariable<bool> isPossessed = new NetworkVariable<bool>(false);
    [SerializeField] protected TruckArea truckArea;


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
        if (Instance == null)
            Instance = this;
    }



    public void quitShop()
    {
        CanvasInGame.Instance.showShop(false);
                
        if (_playerController.IsLocalPlayer)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            _playerController.enabled = true;
            isPossessed.Value = false;
        }
    }

    public void Interact(PlayerController other)
    {
        if (isPossessed.Value) return;



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

        CanvasInGame.Instance.showShop(true);

        if (other.IsLocalPlayer)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _playerController = other;
            other.enabled = false;
            isPossessed.Value = true;
        }


    }
}