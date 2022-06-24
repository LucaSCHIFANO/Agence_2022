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
        
        
        _playerController.ChangeInputHandler(PossessingType.CHARACTER, gameObject);
        CanvasInGame.Instance.showGen(false);
        
        if (_playerController.Object.HasInputAuthority)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            _playerController.enabled = true;
            isPossessed = false;
        }

    }



    public void Interact(NetworkedPlayer other)
    {
        if (isPossessed) return;

        if (other.Object.HasInputAuthority)
        {
            other.ChangeInputHandler(PossessingType.NONE, gameObject);
            CanvasInGame.Instance.showGen(true);
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _playerController = other;
            other.enabled = false;
            isPossessed = true;
        }


    }
}
