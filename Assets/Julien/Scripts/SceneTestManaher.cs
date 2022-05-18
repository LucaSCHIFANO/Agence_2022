using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SceneTestManaher : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnect;
    }

    private void HandleClientConnect(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId) { return; }
     
        Debug.Log("Client connect");
        
        NetworkManager.Singleton.SceneManager.OnSceneEvent += SceneManagerOnOnSceneEvent;
    }
    

    private void SceneManagerOnOnSceneEvent(SceneEvent sceneevent)
    {
        Debug.Log("SCENE EVENT !!!");
        Debug.Log(sceneevent.SceneEventType);
    }
}
