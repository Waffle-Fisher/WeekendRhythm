using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BeatCountUpdater : MonoBehaviour
{
    public static BeatCountUpdater Instance { get; private set; }
    TextMeshProUGUI countText;

    private readonly string s = "Beat count: ";

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        countText = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateText(int i)
    {
        countText.text = s + i;
    }
}
