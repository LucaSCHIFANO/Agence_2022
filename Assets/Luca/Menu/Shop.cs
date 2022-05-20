using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.Jobs;
using UnityEngine;

public class Shop : NetworkBehaviour
{
    private PlayerController _playerController;
    [Networked] private bool isPossessed { get; set; }
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
                
        if (_playerController.Object.HasInputAuthority)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            _playerController.enabled = true;
            isPossessed = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isPossessed) return;
        
        PlayerController playerController;
        
        if (other.gameObject.TryGetComponent(out playerController))
        {
            var player = other.gameObject.GetComponent<PlayerController>();
            
            if (Input.GetKey(KeyCode.E))
            {
                var okay = false;
                foreach (var VARIABLE in truckArea.objectInAreaTruck)
                {
                    if (VARIABLE.gameObject.tag == "Car")
                    {
                        okay = true;
                        break;
                    }
                }
                
                if(!okay) return;

                CanvasInGame.Instance.showShop(true);
                
                if (player.Object.HasInputAuthority)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;

                    _playerController = player;
                    player.enabled = false;
                    isPossessed = true;
                }
            }
        }
    }
}
