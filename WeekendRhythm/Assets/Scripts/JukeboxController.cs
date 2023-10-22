using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JukeboxController : MonoBehaviour
{
    public static JukeboxController Instance;
    private AudioSource m_AudioSource;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    // Start is called before the first frame update
    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public IEnumerator PlaySong(float delay)
    {
        yield return new WaitForSeconds(delay);
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
    }
}
