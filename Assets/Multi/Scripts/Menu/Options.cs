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
        Hide();
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
        
        if(!isMainMenu) Runner.GetPlayerObject(Runner.LocalPlayer).GetComponent<NetworkCharacterControllerPrototypeCustom>().changeSensi();
        else Debug.Log("blablanalanalnzlanalanlan");
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
        
    }
}
