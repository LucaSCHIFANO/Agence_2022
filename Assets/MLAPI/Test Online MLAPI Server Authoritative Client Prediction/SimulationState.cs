using System.Data.SqlTypes;
using Unity.Netcode;
using UnityEngine;

public struct SimulationState : INetworkSerializable
{
    public Vector3 position;
    public Vector3 velocity;
    public int simulationFrame;
    
    public bool initialized;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref velocity);
        serializer.SerializeValue(ref simulationFrame);
        serializer.SerializeValue(ref initialized);
    }
}