using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMenu : MonoBehaviour
{
    
    [SerializeField] private LobbyPanel _lobbyPanel;
    
    private void Awake()
    {
        _lobbyPanel.Hide();
    }
    
    public void OnPlay()
    {
        _lobbyPanel.Show();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
