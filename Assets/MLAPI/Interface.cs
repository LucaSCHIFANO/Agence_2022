using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Interface : MonoBehaviour
{
    static string code;

    public static RelayHostData hostData;

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }

        GUILayout.EndArea();
    }

    static async void StartButtons()
    {
        if (GUILayout.Button("Host"))
        {
            if (RelayManager.instance.IsRelayEnabled)
            {
                hostData = await RelayManager.instance.SetupRelay();
                    
            }
            NetworkManager.Singleton.StartHost();
        }
        
        code = GUILayout.TextField(code);
        if (GUILayout.Button("Client") && !string.IsNullOrEmpty(code))
        {
            if (RelayManager.instance.IsRelayEnabled)
            {
                await RelayManager.instance.JoinRelay(code);
            }
            NetworkManager.Singleton.StartClient();
        }
    }

    static void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ? "Host" :
            NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
                        NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
            
        if (NetworkManager.Singleton.IsHost)
        {
            GUILayout.Label("Join Code: " + hostData.JoinCode);
        }
    }
}
