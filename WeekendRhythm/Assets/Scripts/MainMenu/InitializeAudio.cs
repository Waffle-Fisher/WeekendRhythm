using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class InitializeAudio : MonoBehaviour
{
    public Slider MusicSlider;
    public Slider SFXSlider;
    public SettingsScriptableObject settingValues;
    [SerializeField]
    private float musicDelay;
    [SerializeField]
    private AudioClip audioClip;

    void Start() {
        MusicSlider.value = settingValues.MusicVolumePercent;
        SFXSlider.value = settingValues.SFXVolumePercent;
        if(audioClip)
        {
            JukeboxController.Instance.GetComponent<AudioSource>().clip = audioClip;
            StartCoroutine(JukeboxController.Instance.PlaySong(musicDelay));
        }
    }
}
