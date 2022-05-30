using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GenPanel : NetworkBehaviour
{
    
    [Header("Interact")]
    private NetworkedPlayer _playerController;
    [Networked] private bool isPossessed { get; set; }
    
    
    
    #region Singleton

    private static GenPanel instance;

    public static GenPanel Instance
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



    public void quitPanel()
    {
        CanvasInGame.Instance.showGen(false);
                
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
        
        NetworkedPlayer playerController;
        
        if (other.gameObject.TryGetComponent(out playerController))
        {
            var player = other.gameObject.GetComponent<NetworkedPlayer>();
            
            if (Input.GetKey(KeyCode.E))
            {
                CanvasInGame.Instance.showGen(true);
                
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
