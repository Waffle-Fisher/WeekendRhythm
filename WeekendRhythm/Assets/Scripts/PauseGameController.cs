using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGameController : MonoBehaviour
{
    public static PauseGameController Instance {get; private set;}

    [SerializeField]
    private GameObject settingsUI;

    public bool IsPaused {get; private set;} = false;
    private float originalTimeScale = 1f;


    public void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void Pause()
    {
        originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        JukeboxController.Instance.AudioSource.Pause();
        Debug.Log(Time.timeScale);
        IsPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = originalTimeScale;
        JukeboxController.Instance.AudioSource.UnPause();
        Debug.Log(Time.timeScale);
        IsPaused = false;
    }

    public bool IsSettingsActive() { return settingsUI.activeSelf;}

    public void ToggleSettingsMenu()
    {
        settingsUI.SetActive(!IsSettingsActive());
    }

}
