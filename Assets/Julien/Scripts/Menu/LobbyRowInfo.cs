using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyRowInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyName;
    [SerializeField] private TextMeshProUGUI _slotsInfo;
    
    private Lobby lobby;

    public void SetLobbyInfo(Lobby lobby)
    {
        this.lobby = lobby;

        _lobbyName.text = lobby.Name;
        _slotsInfo.text = $"{lobby.Players.Count}/{lobby.MaxPlayers} Joueurs";
    }

    public async void JoinLobby()
    {
        Lobby joinedLobby = await LobbyManager.instance.JoinLobby(lobbyId: lobby.Id);
        EventSystem.JoinLobbyEvent(joinedLobby);
    }
}
