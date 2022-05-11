using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnPlayer : NetworkBehaviour
{
    public static SpawnPlayer instance;
    
    private void Awake()
    {
        if (instance != null) Destroy(this);
        instance = this;
    }

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<Transform> SpawnPossible;
    [SerializeField] private List<Material> skinColor;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Debug.Log("Network Spawn 'SpawnPlayer'");
        
        if (IsServer)
        {
            
            Debug.Log("Is Server true");
            int i = 0;
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                Debug.Log("Spawning Car for client " + clientId);

                GameObject car = Instantiate(playerPrefab, SpawnPossible[i].position, SpawnPossible[i].rotation);
                car.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
                i++;
            }
        }
    }
}
