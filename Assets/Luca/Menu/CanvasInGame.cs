using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasInGame : MonoBehaviour
{
    [Header("Overheat")]
    public GameObject overHeatDisplay;
    public Image overheatSlider;

    [Header("Shop")] 
    public GameObject shopDisplay;
    
    [Header("Generator")] 
    public GameObject genDisplay;

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
}
