using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerRowDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text displayNameText;
    
    /*private NetworkVariable<FixedString32Bytes> displayName = new NetworkVariable<FixedString32Bytes>();

    private void Awake()
    {
        Debug.Log("Enable PlayerRowDisplay");
        displayName.OnValueChanged += HandleDisplayNameChanged;
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("Network Spawn");
        if (!IsServer) return;

        transform.SetParent(MainMenu.instance._InLobbyList);
        transform.localScale = Vector3.one;
        GetComponent<RectTransform>().localPosition =
            new Vector3(GetComponent<RectTransform>().localPosition.x, GetComponent<RectTransform>().localPosition.y, 0);

        PlayerData? playerData = RelayManager.GetPlayerData(OwnerClientId);
        
        if (playerData.HasValue)
        {
            displayName.Value = playerData.Value.Username;
            Debug.Log("Username Changed");
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        
        displayName.OnValueChanged -= HandleDisplayNameChanged;
        Debug.Log("Disable Rows");
    }

    void HandleDisplayNameChanged(FixedString32Bytes oldDisplayName, FixedString32Bytes newDisplayName)
    {
        Debug.Log(newDisplayName);
        displayNameText.text = newDisplayName.ToString();
    }*/
    public void DisplayPlayerName(FixedString64Bytes playerName)
    {
        Debug.Log("DisplayName " + playerName);
        displayNameText.text = playerName.ToString();
    }
}
