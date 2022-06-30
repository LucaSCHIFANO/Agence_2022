using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Fusion;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkedPlayer : NetworkBehaviour
{
    public GameObject Camera;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private List<GameObject> _playerVisuals;


    [Networked] public NetworkBool IsInSomething { get; set; }

    Rewired.Player playerRew;

    public static NetworkedPlayer Local { get; set; }
    public PossessingType PossessingType = PossessingType.CHARACTER;


    public CharacterMovementHandler CharacterMovementHandler;
    public CharacterInputHandler CharacterInputHandler;
    public WeaponInputHandler WeaponInputHandler;
    public VehiculeInputHandler VehiculeInputHandler;

    private PlayerInteraction _playerInteraction;
    private Player _player;
    private bool isPaused;

    public override void Spawned()
    {
        _player = App.Instance.GetPlayer(Object.InputAuthority);
        _name.text = _player.Name;
        _name.color = _player.Color;
        
        _playerInteraction = GetComponent<PlayerInteraction>();
        CharacterInputHandler = GetComponent<CharacterInputHandler>();
        CharacterMovementHandler = GetComponent<CharacterMovementHandler>();
        //playerRew = Rewired.ReInput.players.GetPlayer(0);
        if (Object.HasInputAuthority)
        {
            Local = this;
            Debug.Log("Spawned Local Player");
            Camera.SetActive(true);
            HideSelfVisual();
            
        }
        else
        {
            Debug.Log("Spawned Remote Player");
        }

        CanvasInGame.Instance.showOptiones(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            CanvasInGame.Instance.showOptiones(isPaused);

            if (isPaused)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        
    }

    public void Unpossess(Transform exitPoint)
    {
        if (Object.HasInputAuthority)
            Camera.SetActive(true);
        
        transform.position = exitPoint.position;
        transform.rotation = Quaternion.identity;

        GetComponent<CharacterController>().enabled = true;
        //isSomethingActuRpc(false);
    }

    public void Possess(Transform seat)
    {
        GetComponent<CharacterController>().enabled = false;

        if (Runner.IsServer)
        {
            transform.position = seat.position;
            transform.rotation = seat.rotation;
        }
        
        if (Object.HasInputAuthority)
            Camera.SetActive(false);

        //isSomethingActuRpc(true);
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void isSomethingActuRpc(NetworkBool leBool)
    {
        IsInSomething = leBool;
        Debug.Log("first step");
    }
    
    
    public void ChangeInputHandler(PossessingType possessingType, GameObject handler)
    {
        PossessingType = possessingType;
        _playerInteraction.HideTooltip();
        if (possessingType == PossessingType.WEAPON)
        {
            WeaponInputHandler = handler.GetComponent<WeaponInputHandler>();
            _playerInteraction.enabled = false;
        }
        else if (possessingType == PossessingType.CAR)
        {
            VehiculeInputHandler = handler.GetComponent<VehiculeInputHandler>();
            _playerInteraction.enabled = false;
        }
        else if (possessingType == PossessingType.CHARACTER)
        {
            CharacterInputHandler = GetComponent<CharacterInputHandler>();
            WeaponInputHandler = null;
            VehiculeInputHandler = null;
            _playerInteraction.enabled = true;
            _playerInteraction.ShowTooltip();
        }
        else
        {
            _playerInteraction.enabled = false;
            CharacterInputHandler = null;
            WeaponInputHandler = null;
            VehiculeInputHandler = null;
        }
    }

    

    public void HideSelfVisual()
    {
        _playerVisuals.ForEach(el => el.SetActive(false));
    }

    public void ShowSelfVisual()
    {
        _playerVisuals.ForEach(el => el.SetActive(true));
    }
}

public enum PossessingType
{
    CHARACTER,
    CAR,
    WEAPON,
    NONE
}