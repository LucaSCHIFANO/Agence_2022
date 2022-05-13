using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class ScrapMetal : NetworkBehaviour
{
    public int scrap;
    public TextMeshProUGUI textSmetals;

    private NetworkVariable<int> scrapLeft = new NetworkVariable<int>();

    #region Singleton
    private static ScrapMetal instance;
    public static ScrapMetal Instance { get => instance; set => instance = value; }
    #endregion

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        scrapLeft.Value = scrap;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsClient)
        {
            scrapLeft.OnValueChanged += OnScrapChanged;
        }
        
    }

    private void OnScrapChanged(int previousvalue, int newvalue)
    {
        actuText();
        scrap = newvalue;
        Debug.Log($"Scrap : {scrap} | Server Scrap : {scrapLeft.Value}");
    }


    private void Start()
    {
        actuText();
    }

    [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
    public void addMoneyServerRpc(int lint)
    {
        scrapLeft.Value += lint;
    }

    public void actuText()
    {
        textSmetals.text = "Metals : " + scrapLeft.Value.ToString();
    }
}
