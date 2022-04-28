using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrapMetal : MonoBehaviour
{
    public int scrap;
    public TextMeshProUGUI textSmetals;
    
    #region Singleton
    private static ScrapMetal instance;
    public static ScrapMetal Instance { get => instance; set => instance = value; }
    #endregion

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    
    
    private void Start()
    {
        actuText();
    }

    public void addMoney(int lint)
    {
        scrap += lint;
        actuText();
    }

    public void actuText()
    {
        textSmetals.text = "Metals : " + scrap.ToString();
    }
}
