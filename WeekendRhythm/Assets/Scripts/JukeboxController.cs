using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JukeboxController : MonoBehaviour
{
    public static JukeboxController Instance;
    public AudioSource AudioSource { get; private set; }
    public SettingsScriptableObject settingValues;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
        AudioSource = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(this.gameObject);
        AudioSource.volume = settingValues.MusicVolumePercent;
    }

    public IEnumerator PlaySong(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioSource.PlayOneShot(AudioSource.clip);
    }

    public void ChangeVolume(float i)
    {
        AudioSource.volume = i;
    }
}
