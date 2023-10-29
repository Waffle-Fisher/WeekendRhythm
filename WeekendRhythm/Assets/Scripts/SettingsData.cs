using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class SettingsData : MonoBehaviour
{
    [Range(0f, 1f)]
    public float MusicVolumePercent = 0.5f;
    [Range(0f, 1f)]
    public float SFXVolumePercent = 0.5f;
    [SerializeField]
    [Min(1)]
    [Tooltip("Should be the same as slider's max value")]
    private int maxVolume = 10;

    private bool isMusic = false;
    private bool isSFX = false;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }


    public void SetVolume(float newVolume)
    {
        if(isMusic && isSFX) { 
            Debug.LogError("Both isMusic and isSFX are true");
            return;
        }
        if (isMusic) { MusicVolumePercent = newVolume; 
            Debug.Log("MusicVolume: " + MusicVolumePercent);
        }
        else if(isSFX) { SFXVolumePercent = newVolume;
            Debug.Log("SFXVolume: " + SFXVolumePercent);
        }
        else {
            Debug.LogError("Both isMusic and isSFX are false");
            return;
        }
    }

    public void SetIsMusic(bool b)
    {
        isMusic = b;
    }

    public void SetIsSFX(bool b)
    {
        isSFX = b;
    }
}
