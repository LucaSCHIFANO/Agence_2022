using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DebugSpawnWeapon : NetworkBehaviour
{
    [SerializeField] private GameObject weaponPrefab;
    
    void Start()
    {

        if (IsServer)
        {
            GameObject prefabGO = Instantiate(weaponPrefab, transform);
            prefabGO.GetComponent<NetworkObject>().Spawn();
        }

    }
}
