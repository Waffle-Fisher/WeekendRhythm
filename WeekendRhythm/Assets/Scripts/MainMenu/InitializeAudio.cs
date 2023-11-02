using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InitializeAudio : MonoBehaviour
{
    public Slider MusicSlider;
    public Slider SFXSlider;
    public SettingsScriptableObject settingValues;
    void Start() {
        MusicSlider.value = settingValues.MusicVolumePercent;
        SFXSlider.value = settingValues.SFXVolumePercent;
    }
}
