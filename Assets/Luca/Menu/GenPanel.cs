using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GenPanel : NetworkBehaviour
{
    
    [Header("Interact")]
    private PlayerController _playerController;
    private NetworkVariable<bool> isPossessed = new NetworkVariable<bool>(false);
    
    
    
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


        CanvasInGame.Instance.showGen(true);

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
