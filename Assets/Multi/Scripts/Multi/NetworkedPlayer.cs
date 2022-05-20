using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkedPlayer : NetworkBehaviour/*, IPlayerLeft*/
{
    [SerializeField] private GameObject Camera;
    public static NetworkedPlayer Local { get; set; }

    [SerializeField] private CharacterInputHandler _inputHandler;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            Debug.Log("Spawned Local Player");
            Camera.SetActive(true);
        }
        else
        {
            Debug.Log("Spawned Remote Player");
        }
    }

    /*public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority) Runner.Despawn(Object);
    }*/
    
    
    public void Unpossess(Transform exitPoint)
    {
        if (Object.HasInputAuthority)
            Camera.SetActive(true);
        
        transform.position = exitPoint.position;
        transform.rotation = exitPoint.rotation;
        
        GetComponent<CharacterController>().enabled = true;
        // _inputHandler.enabled = true;
    }

    public void Possess(Transform seat)
    {
        GetComponent<CharacterController>().enabled = false;
        // _inputHandler.enabled = false;

        if (Runner.IsServer)
        {
            transform.position = seat.position;
            transform.rotation = seat.rotation;
        }

        if (Object.HasInputAuthority)
            Camera.SetActive(false);
    }
}
