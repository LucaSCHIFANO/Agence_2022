using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [SerializeField] AudioMixer audioMix;
    [SerializeField] Slider[] sliderList;

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
    
    private void saveSlider(string nameVol, float volume)
    {
        PlayerPrefs.SetFloat(nameVol, volume);
    }

    private void getSlider()
    {
        sliderList[0].value = PlayerPrefs.GetFloat("MasterVol");
        sliderList[1].value = PlayerPrefs.GetFloat("MusicVol");
        sliderList[2].value = PlayerPrefs.GetFloat("SFXVol");
    }
}
