using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JukeboxController : MonoBehaviour
{
    public static JukeboxController Instance;
    public AudioSource AudioSource { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    // Start is called before the first frame update
    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    public IEnumerator PlaySong(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioSource.PlayOneShot(AudioSource.clip);
    }
}
