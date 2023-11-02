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
    [SerializeField]
    private float musicDelay;


    void Start() {
        MusicSlider.value = settingValues.MusicVolumePercent;
        SFXSlider.value = settingValues.SFXVolumePercent;
        StartCoroutine(JukeboxController.Instance.PlaySong(musicDelay));
    }
}
