using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct ClientInputState : INetworkSerializable
{
    public Vector2 pressedInput;
    // public float rotationX;
    public bool jumped;
    public int simulationFrame;

    public bool initialized;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref pressedInput);
        // serializer.SerializeValue(ref rotationX);
        serializer.SerializeValue(ref jumped);
        serializer.SerializeValue(ref simulationFrame);
        serializer.SerializeValue(ref initialized);
    }
}
