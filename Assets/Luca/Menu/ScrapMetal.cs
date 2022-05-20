using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrapMetal : NetworkBehaviour
{
    public int scrap;
    public TextMeshProUGUI textSmetals;

    [Networked] private int scrapLeft { get; set; }

    #region Singleton
    private static ScrapMetal instance;
    public static ScrapMetal Instance { get => instance; set => instance = value; }
    #endregion

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        scrapLeft = scrap;
    }

    private void OnScrapChanged(int previousvalue, int newvalue)
    {
        actuText();
        scrap = newvalue;
        Debug.Log($"Scrap : {scrap} | Server Scrap : {scrapLeft}");
    }


    private void Start()
    {
        actuText();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void addMoneyServerRpc(int lint)
    {
        scrapLeft += lint;
    }

    public void actuText()
    {
        textSmetals.text = "Metals : " + scrapLeft;
    }
}
