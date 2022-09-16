using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SpawnerVehicule : NetworkBehaviour
{
    [SerializeField] private List<Transform> teleportPoints;
    [SerializeField] private NetworkObject VehiculeToSpawn;
    [SerializeField] private GameObject enemyToSpawn;

    public static SpawnerVehicule instance;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        instance = this;
    }

    private void Update()
    {
        if (Runner != null && Runner.IsServer && Input.GetKeyDown(KeyCode.O))
        {
            SpawnEnemy();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void SpawnEnemy(RpcInfo info = default)
    {
        NetworkObject spawned = Runner.Spawn(enemyToSpawn.GetComponent<NetworkObject>(), new Vector3(0f, 1f, 0f));
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void SpawnCar(RpcInfo info = default)
    {
        GameObject obj = Runner.GetPlayerObject(info.Source).gameObject;
        NetworkObject spawned = Runner.Spawn(VehiculeToSpawn, obj.transform.position + new Vector3(0, 2, 0));
        spawned.GetComponent<TruckPhysics>().teleport = teleportPoints;
    }
}
