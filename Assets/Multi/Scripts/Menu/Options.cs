using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Options : NetworkBehaviour
{
    [SerializeField] AudioMixer audioMix;
    [SerializeField] Slider[] sliderList;

    [SerializeField] private bool isMainMenu;

    private void Start()
    {
        getSlider();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetSlider1(float volume)
    {
        audioMix.SetFloat("MasterVol", volume);
        saveSlider("MasterVol", volume);
    }

    public void SetSlider2(float volume)
    {
        audioMix.SetFloat("MusicVol", volume);
        saveSlider("MusicVol", volume);
    }

    public void SetSlider3(float volume)
    {
        audioMix.SetFloat("SFXVol", volume);
        saveSlider("SFXVol", volume);
    }

    public void SetSlider4(float sensibility)
    {
        saveSlider("Sensi", sensibility);
        if(!isMainMenu) Runner?.GetPlayerObject(Runner.LocalPlayer).GetComponent<NetworkCharacterControllerPrototypeCustom>().changeSensi();
    }
    
    private void saveSlider(string nameVol, float volume)
    {
        PlayerPrefs.SetFloat(nameVol, volume);
    }

    private void getSlider()
    {
        sliderList[0].value = PlayerPrefs.GetFloat("MasterVol");
        sliderList[1].value = PlayerPrefs.GetFloat("MusicVol");
        sliderList[2].value = PlayerPrefs.GetFloat("SFXVol");
        sliderList[3].value = PlayerPrefs.GetFloat("Sensi");
        
        if(sliderList[3].value < sliderList[3].minValue) sliderList[3].value = sliderList[3].minValue;
        
        audioMix.SetFloat("MasterVol", sliderList[0].value);
        audioMix.SetFloat("MusicVol", sliderList[1].value);
        audioMix.SetFloat("SFXVol", sliderList[2].value);
        audioMix.SetFloat("Sensi", sliderList[3].value);
        
        if(isMainMenu) Hide();
    }
}
