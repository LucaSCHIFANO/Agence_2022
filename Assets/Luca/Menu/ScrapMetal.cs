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
    
    // (OnChanged = nameof(OnScrapChanged))
    [Networked] private int scrapLeft { get; set; }

    #region Singleton
    private static ScrapMetal instance;
    public static ScrapMetal Instance { get => instance; set => instance = value; }
    #endregion

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public override void Spawned()
    {
        base.Spawned();
        scrapLeft = scrap;
        
        actuText();
    }

    private void OnScrapChanged(ScrapMetal changed)
    {
        actuText();
        scrap = changed.scrapLeft;
        Debug.Log($"Scrap : {scrap} | Server Scrap : {scrapLeft}");
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
