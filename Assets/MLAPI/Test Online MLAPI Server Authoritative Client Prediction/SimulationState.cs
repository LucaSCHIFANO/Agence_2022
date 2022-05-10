using Unity.Netcode;
using UnityEngine;

public class SimulationState : INetworkSerializable
{
    public Vector3 position;
    public Vector3 velocity;
    public int simulationFrame;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref velocity);
        serializer.SerializeValue(ref simulationFrame);
    }
}