using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private List<Transform> spawnPoint;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Debug.Log("Network Spawn 'SpawnPlayer'");
        
        if (IsServer)
        {
            int i = 0;
            Debug.Log("Is Server true");
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                Debug.Log("Spawning Car for client " + clientId);

                GameObject car = Instantiate(playerPrefab,spawnPoint[i]);
                car.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
                i++;
            }

        }
    }
}
