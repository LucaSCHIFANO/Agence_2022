using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _error;
    [SerializeField] private Transform _lobbyListParent;
    [SerializeField] private LobbyRowInfo _lobbyRowPF;
    [SerializeField] private TMP_InputField _lobbyCode;
    [SerializeField] private CreationLobbyPanel _creationLobbyPanel;
    
    public async void Show()
    {
        gameObject.SetActive(true);
        _error.text = "";
        OnSessionListUpdated(new List<SessionInfo>());
        await App.Instance.EnterLobby($"LobbyListMaster", OnSessionListUpdated);
    }

    public void Hide()
    {
        _creationLobbyPanel.Hide();
        gameObject.SetActive(false);
        App.Instance.Disconnect();
    }
    
    public void OnSessionListUpdated(List<SessionInfo> sessions)
    {
        for (int  i = 0;  i < _lobbyListParent.childCount;  i++)
        {
            Destroy(_lobbyListParent.GetChild(i).gameObject);
        }
        
        if (sessions == null)
        {
            Hide();
            _error.text = "Failed to join lobby";
            return;
        }
        
        if (sessions.Count <= 0)
        {
            _error.text = "No Lobbies Found";
            _error.gameObject.SetActive(true);
            return;
        }
        
        _error.gameObject.SetActive(false);
        
        foreach (SessionInfo info in sessions)
        {
            if (info.IsVisible)
            {
                LobbyRowInfo rowInfo = Instantiate(_lobbyRowPF, _lobbyListParent);
                rowInfo.Setup(info);
            }
        }
    }
    
    public void OnShowNewSessionUI()
    {
        _creationLobbyPanel.Show();
    }

    public void JoinSession()
    {
        App.Instance.JoinSessionByCode(_lobbyCode.text);
    }

}
