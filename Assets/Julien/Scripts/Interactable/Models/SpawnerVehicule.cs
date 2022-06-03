using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SpawnerVehicule : NetworkBehaviour
{
    [SerializeField] private List<Transform> teleportPoints;
    [SerializeField] private NetworkObject VehiculeToSpawn;

    public static SpawnerVehicule instance;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        instance = this;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void SpawnCar(RpcInfo info = default)
    {
        GameObject obj = Runner.GetPlayerObject(info.Source).gameObject;
        NetworkObject spawned = Runner.Spawn(VehiculeToSpawn, obj.transform.position + new Vector3(0, 2, 0));
        spawned.GetComponent<TruckPhysics>().teleport = teleportPoints;
    }
}
