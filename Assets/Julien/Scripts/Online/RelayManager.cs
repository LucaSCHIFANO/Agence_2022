using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    public static RelayManager instance;
    private Dictionary<ulong, PlayerData> ClientData;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.Shutdown();
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }

    public bool IsRelayEnabled =>
        Transport != null && Transport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport;
    
    public UnityTransport Transport => NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

    public async Task<RelayHostData> SetupRelay(int maxConnections, string playerName, int choosenColor)
    {
        Debug.Log("Relay Server Starting...");

        Allocation allocation = await Relay.Instance.CreateAllocationAsync(maxConnections);

        RelayHostData relayHostData = new RelayHostData
        {
            Key = allocation.Key,
            Port = (ushort)allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            IPv4Address = allocation.RelayServer.IpV4,
            ConnectionData = allocation.ConnectionData
        };

        relayHostData.JoinCode = await Relay.Instance.GetJoinCodeAsync(relayHostData.AllocationID);
        
        Transport.SetRelayServerData(relayHostData.IPv4Address, relayHostData.Port, relayHostData.AllocationIDBytes, relayHostData.Key, relayHostData.ConnectionData);
        
        Debug.Log("Relay Server Started Join Code : " + relayHostData.JoinCode);

        ClientData = new Dictionary<ulong, PlayerData>();
        ClientData[NetworkManager.Singleton.LocalClientId] = new PlayerData(playerName, choosenColor);
        Debug.Log(ClientData);


        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        
        return relayHostData;
    }

    public async Task<RelayJoinData> JoinRelay(string joinCode, string userName, int choosenColor)
    {
        JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        
        RelayJoinData relayJoinData = new RelayJoinData
        {
            IPv4Address = allocation.RelayServer.IpV4,
            Port = (ushort) allocation.RelayServer.Port,

            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            ConnectionData = allocation.ConnectionData,
            HostConnectionData = allocation.HostConnectionData,
            Key = allocation.Key,
        };
        
        Transport.SetRelayServerData(relayJoinData.IPv4Address, relayJoinData.Port, relayJoinData.AllocationIDBytes, relayJoinData.Key, relayJoinData.ConnectionData, relayJoinData.HostConnectionData);
        
        Debug.Log("Client joined relay with code : " + joinCode);
        var payload = JsonUtility.ToJson(new ConnectionPayload()
        {
            userName = userName,
            choosenColor = choosenColor
        });

        byte[] payloadBytes = Encoding.ASCII.GetBytes(payload);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
        
        return relayJoinData;
    }

    void HandleClientDisconnect(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            ClientData.Remove(clientId);
        }
    }

    public static PlayerData? GetPlayerData(ulong clientId)
    {
        if (instance.ClientData.TryGetValue(clientId, out PlayerData playerData)) return playerData;

        return null;
    }
    
    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        string payload = Encoding.ASCII.GetString(connectionData);

        var connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);

        bool connectionApproved = connectionPayload != null;

        if (connectionApproved)
        {
            Debug.Log(connectionPayload);
            ClientData[clientId] = new PlayerData(connectionPayload.userName, connectionPayload.choosenColor);
        }

        callback(false, null, connectionApproved, null, null);
    }
    
}
