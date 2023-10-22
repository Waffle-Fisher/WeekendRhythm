using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    TextMeshProUGUI text;
    int minutes = 0;
    int seconds = 0;
    float time = 0f;
    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    // Update is called once per frame
    void Update()
    {
        time = BeatMapHandler.Instance.TimeSinceStart;
        minutes = (int)(time / 60);
        seconds = (int)(time % 60);
        Debug.Log("Minutes: " + minutes);
        Debug.Log("Seconds: " + seconds);
        text.text = string.Format("{0}:{1:00}", minutes, seconds);
    }
}
