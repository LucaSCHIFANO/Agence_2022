using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasInGame : MonoBehaviour
{
    [Header("Overheat")] public GameObject overHeatDisplay;
    public Image overheatSlider;

    [Header("Shop")] public GameObject shopDisplay;

    [Header("Generator")] public GameObject genDisplay;

    [Header("Health")] public Image healthDisplay;

    [Header("Options")] public GameObject optDisplay;
    
    [Header("Overheat")] public GameObject truckDisplay;
    public Image fuelSlider;
    
    #region Singleton

    private static CanvasInGame instance;

    public static CanvasInGame Instance
    {
        get => instance;
        set => instance = value;
    }

    #endregion

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void showOverheat(bool boul)
    {
        overHeatDisplay.SetActive(boul);
    }

    public void showShop(bool boul)
    {
        shopDisplay.SetActive(boul);
    }

    public void showGen(bool boul)
    {
        genDisplay.SetActive(boul);
    }

    public void showBlood(bool boul)
    {
        healthDisplay.gameObject.SetActive(boul);
    }

    public void actuBlood(float alpha)
    {
        var col = healthDisplay.color;
        healthDisplay.color = new Color(col.r, col.g, col.b, alpha);
    }
    
    public void showOptiones(bool boul)
    {
        optDisplay.SetActive(boul);
    }
    
    public void showTruck(bool boul)
    {
        truckDisplay.SetActive(boul);
    }
}
