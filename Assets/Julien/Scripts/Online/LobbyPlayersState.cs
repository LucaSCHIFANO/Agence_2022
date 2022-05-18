using System;
using Unity.Collections;
using Unity.Netcode;

public struct LobbyPlayersState : INetworkSerializable, IEquatable<LobbyPlayersState>
{

    public ulong ClientId;
    public FixedString64Bytes PlayerName;
    public bool IsReady;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref IsReady);
    }

    public bool Equals(LobbyPlayersState other)
    {
        return ClientId == other.ClientId && PlayerName.Equals(other.PlayerName) && IsReady == other.IsReady;
    }

    public override bool Equals(object obj)
    {
        return obj is LobbyPlayersState other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ClientId, PlayerName, IsReady);
    }
}
