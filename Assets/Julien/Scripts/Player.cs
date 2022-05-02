using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private GameObject camera;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        Debug.Log("Spawned in game");

        if (IsOwner)
        {
            camera.SetActive(true);
        }
    }
}
