using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsData : MonoBehaviour
{
    public static SettingsData Instance { get; private set; }

    public SettingsScriptableObject settingValues;

    // [Range(0f, 1f)]
    // public float MusicVolumePercent= 0.5f;// = settingValues.MusicVolumePercent;
    // [Range(0f, 1f)]
    // public float SFXVolumePercent = 0.5f;

    private void Awake()
    {
        if(Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetMusicVolume(float newVolume)
    {
        settingValues.MusicVolumePercent = (float)Math.Round(newVolume,2); 
        Debug.Log("MusicVolume: " + settingValues.MusicVolumePercent);
    }

    public void SetSFXVolume(float newVolume)
    {
        settingValues.SFXVolumePercent = (float)Math.Round(newVolume,2);
        Debug.Log("SFXVolume: " + settingValues.SFXVolumePercent);
    }

    public void ChangeMusicText(GameObject g)
    {
        g.GetComponent<TextMeshProUGUI>().text = String.Format("{0:0%}",settingValues.MusicVolumePercent);
    }

    public void ChangeSFXText(GameObject g)
    {
        g.GetComponent<TextMeshProUGUI>().text = String.Format("{0:0%}",settingValues.SFXVolumePercent);
    }
}