using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public List<GameObject> GOs = new List<GameObject>();
    public void Play()
    {
        GOs[0].SetActive(false);
        GOs[1].SetActive(true);
    }

    public void Quit()
    {
        if (Application.isEditor) UnityEditor.EditorApplication.isPlaying = false;
        else Application.Quit();
    }
}
