using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public List<GameObject> GOs = new List<GameObject>();

    public List<GameObject> lobbySwitch = new List<GameObject>();
    public List<Button> lobbyButton = new List<Button>();
    public void Play()
    {
        GOs[0].SetActive(false);
        GOs[1].SetActive(true);
    }

    public void goTOGO(int lint) //change "scene" 
    {
        for (int i = 0; i < GOs.Count; i++)
        {
            if(lint == i) GOs[i].SetActive(true);
            else GOs[i].SetActive(false);
        }
    }
    
    public void switchLobby(int lint) // switch join / create
    {
        for (int i = 0; i < lobbySwitch.Count; i++)
        {
            if (lint == i)
            {
                lobbySwitch[i].SetActive(true);
                lobbyButton[i].image.color = new Color(0, 0, 0, 0.7019608f);
            }
            else
            {
                lobbySwitch[i].SetActive(false);
                lobbyButton[i].image.color = new Color(0, 0, 0, 0.4f);
            }
        }
    }

    public void Quit()
    {
        if (Application.isEditor) UnityEditor.EditorApplication.isPlaying = false;
        else Application.Quit();
    }

    public void Refresh()
    {
        Debug.Log("La il faut refresh julien");
    }
    
    public void Join()
    {
        Debug.Log("Techniquement ca join un lobby si yen a un selectionné sinon rip");
    }
    
    public void Create()
    {
        Application.OpenURL("https://www.youtube.com/watch?v=dpbnVJDip6I");
    }

    public void ready()
    {
        Application.OpenURL("https://www.youtube.com/watch?v=jbSbneyZxVI");
    }
    
}
