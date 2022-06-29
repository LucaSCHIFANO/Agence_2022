using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrapMetal : NetworkBehaviour
{
    [SerializeField] protected int startScrap;
    [SerializeField] protected TextMeshProUGUI textSmetals;
    [SerializeField] protected TextMeshProUGUI textSmetalsInGame;
    
    [HideInInspector] public int scrapLeft;
    [HideInInspector] public int scrapLeftInGame;
    [Networked(OnChanged = nameof(OnScrapChanged))/*, Capacity(1)*/] private int scrapLeftOnline { get; set; }

    #region Singleton
    private static ScrapMetal instance;
    public static ScrapMetal Instance { get => instance; set => instance = value; }
    #endregion

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        //scrapLeftOnline.Set(0, 0);
    }

    public override void Spawned()
    {
        base.Spawned();
        scrapLeft = startScrap;
        
        actuText();
    }

    public static void OnScrapChanged(Changed<ScrapMetal> changed)
    {
        changed.Behaviour.ChangeScrapt();
    }

    private void ChangeScrapt()
    {
        scrapLeft = scrapLeftOnline;
        actuText();
    }
    
    /*private void OnScrapChanged(ScrapMetal changed)
    {
        actuText();
        scrap = changed.scrapLeft;
        Debug.Log($"Scrap : {scrap} | Server Scrap : {scrapLeft}");
    }*/


    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void addMoneyServerRpc(int lint)
    {
        AddServerScrap(lint);
    }

    public void actuText()
    {
        textSmetals.text = "Metals : " + scrapLeft;
        textSmetalsInGame.text = scrapLeft.ToString();
    }

    public void AddServerScrap(int lint)
    {
        scrapLeft += lint;
        scrapLeftOnline = scrapLeft;
    }
}
