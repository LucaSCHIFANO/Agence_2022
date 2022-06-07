using Fusion;
using UnityEngine;

public struct VehiculeInputData : INetworkInput
{
    public Vector2 movement;
    public NetworkBool shiftUp;
    public NetworkBool shiftDown;
    public NetworkBool breaking;
    public NetworkBool shift;
    public NetworkBool isExiting;
    public NetworkBool isHonking;
    public NetworkBool leftControl;
    public NetworkBool teleportToSpawn;
    public NetworkBool teleportToBigDrop;
    public NetworkBool teleportToBigDrop2;
    public NetworkBool teleportToBigDrop3;
    public NetworkBool teleportToBigDrop4;
}