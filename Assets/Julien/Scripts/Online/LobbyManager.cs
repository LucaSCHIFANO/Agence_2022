using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        LeaveLobby();
    }

    public Lobby currentLobby;

    public async Task<Lobby> JoinLobby(string userName, int choosenColor, string lobbyCode = null, string lobbyId = null)
    {
        Lobby lobby = null;
        try
        {
            if (!string.IsNullOrEmpty(lobbyId))
            {
                lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId);
            }
            else if (!string.IsNullOrEmpty(lobbyCode))
            {
                lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode);
            }

            if (lobby!= null)
            {
                string relayCode = lobby.Data["joinCode"].Value;

                await RelayManager.instance.JoinRelay(relayCode, userName, choosenColor);

                NetworkManager.Singleton.StartClient();
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }

        currentLobby = lobby;
        return lobby;
    }

    public async Task<QueryResponse> GetLobbiesList(List<QueryFilter> customFilters, List<QueryOrder> customOrder)
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 100;

            options.Filters = customFilters;

            options.Order = customOrder;

            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

            return lobbies;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }

        return null;
    }

    public async Task<Lobby> QuickJoinLobby(QuickJoinLobbyOptions options, string userName, int choosenColor)
    {
        Lobby lobby = null;
        try
        {
            lobby = await Lobbies.Instance.QuickJoinLobbyAsync(options);

            string relayCode = lobby.Data["joinCode"].Value;

            await RelayManager.instance.JoinRelay(relayCode, userName, choosenColor);

            NetworkManager.Singleton.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Cannot find lobby " + e);
        }

        currentLobby = lobby;
        return lobby;
    }

    public async Task<Lobby> CreateLobby(string lobbyName, int maxPlayer, CreateLobbyOptions lobbyOptions, string userName, int choosenColor)
    {
        try
        {
            RelayHostData relayHostData = await RelayManager.instance.SetupRelay(maxPlayer - 1, userName, choosenColor);

            if (lobbyOptions.Data != null)
            {
                lobbyOptions.Data.Add("joinCode", new DataObject(visibility: DataObject.VisibilityOptions.Member, value: relayHostData.JoinCode));
            }
            else
            {
                lobbyOptions.Data = new Dictionary<string, DataObject>()
                {
                    {
                        "joinCode",
                        new DataObject(
                            visibility: DataObject.VisibilityOptions.Member,
                            value: relayHostData.JoinCode
                        )
                    }
                };
            }
            
            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxPlayer, lobbyOptions);

            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));

            NetworkManager.Singleton.StartHost();

            currentLobby = lobby;
            return lobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }

        return null;
    }

    public Lobby GetCurrentLobby()
    {
        return currentLobby;
    }

    public async Task LeaveLobby()
    {
        if (currentLobby == null) return;
        
        try
        {
            string playerId = AuthenticationService.Instance.PlayerId;

            await Lobbies.Instance.RemovePlayerAsync(currentLobby.Id, playerId);

            Debug.Log("You left the lobby !");
            
            NetworkManager.Singleton.Shutdown();
            
            currentLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async Task RefreshCurrentLobby()
    {
        if (currentLobby == null) return;
        
        try
        {
            currentLobby = await Lobbies.Instance.GetLobbyAsync(currentLobby.Id);
            
            Debug.Log("Lobby Refreshed");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    public async Task<Lobby> UpdateLobbyData(bool locked, string hostId, Dictionary<string, DataObject> others)
    {
        if (currentLobby == null) return null;
        
        try
        {
            UpdateLobbyOptions options = new UpdateLobbyOptions();
            options.IsLocked = locked;
            options.HostId = hostId;
            options.Data = others;

            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(currentLobby.Id, options);
            
            currentLobby = lobby;
            return lobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

        return null;
    }
}