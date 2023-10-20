using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer Instance;

    TextMeshProUGUI text;
    public float TimeSinceSongStarted { get; private set; } = 0f;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    // Update is called once per frame
    void Update()
    {
        TimeSinceSongStarted += Time.deltaTime;
        text.text = TimeSinceSongStarted.ToString("00:00");
    }
}
