using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> m_AudioClipList = new();
    private AudioSource m_AudioSource;

    void Start()
    {
        if(m_AudioClipList.Count == 0) { Debug.LogError("No Player SFX to play"); }
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void PlayAudioClip(int i)
    {
        if(m_AudioSource.isPlaying) { m_AudioSource.Stop(); }
        m_AudioSource.PlayOneShot(m_AudioClipList[i]);
    }
}
