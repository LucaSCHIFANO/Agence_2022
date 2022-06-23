using Fusion;
using TMPro;
using UnityEngine;

public class LobbyRowInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyName;
    [SerializeField] private TextMeshProUGUI _slotsInfo;

    private SessionInfo _info;

    public void Setup(SessionInfo info)
    {
        _info = info;
        
        _lobbyName.text = info.Properties["LobbyName"];
        _slotsInfo.text = $"{info.PlayerCount}/{info.MaxPlayers}";
    }

    public void JoinSession()
    {
        App.Instance.JoinSession(_info);
    }
    
    // public void SetLobbyInfo(/*Lobby lobby*/)
    // {
        // this.lobby = lobby;

        // _lobbyName.text = lobby.Name;
        // _slotsInfo.text = $"{lobby.Players.Count}/{lobby.MaxPlayers} Joueurs";
    // }

    // public async void JoinLobby()
    // {
        // Lobby joinedLobby = await LobbyManager.instance.JoinLobby(PlayerPrefs.GetString("username"), PlayerPrefs.GetInt("skinColor"), lobbyId: lobby.Id);
        // EventSystem.JoinLobbyEvent(joinedLobby);
    // }
}
