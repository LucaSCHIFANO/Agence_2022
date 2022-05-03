using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;

public static class EventSystem
{
    public static event UnityAction<Lobby> OnJoinLobbyEvent;
    public static void JoinLobbyEvent(Lobby lobby) => OnJoinLobbyEvent?.Invoke(lobby);


    public static event UnityAction OnShootEvent;
    public static void ShootEvent() => OnShootEvent?.Invoke();

}
