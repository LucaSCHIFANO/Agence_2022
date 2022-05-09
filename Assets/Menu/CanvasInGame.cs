using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasInGame : MonoBehaviour
{
    public GameObject overHeatDisplay;
    public Image overheatSlider;
    
    
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
}
