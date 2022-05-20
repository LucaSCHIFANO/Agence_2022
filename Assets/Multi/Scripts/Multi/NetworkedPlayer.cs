using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkedPlayer : NetworkBehaviour/*, IPlayerLeft*/
{
    [SerializeField] private GameObject Camera;
    public static NetworkedPlayer Local { get; set; }

    private CharacterInputHandler _inputHandler;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            Debug.Log("Spawned Local Player");
            Camera.SetActive(true);
            _inputHandler = GetComponent<CharacterInputHandler>();
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
        Camera.SetActive(true);
        
        transform.position = exitPoint.position;
        transform.rotation = exitPoint.rotation;
        
        GetComponent<Collider>().enabled = true;
        _inputHandler.enabled = true;
    }

    public void Possess(Transform seat)
    {
        GetComponent<Collider>().enabled = false;
        _inputHandler.enabled = false;
        
        transform.position = seat.position;
        transform.rotation = seat.rotation;
        
        Camera.SetActive(false);
    }
}
