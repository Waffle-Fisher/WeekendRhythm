using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BeatGradeUpdater : MonoBehaviour
{
    public static BeatGradeUpdater Instance { get; private set; }
    TextMeshProUGUI gradeText;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        gradeText = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateText(string text)
    {
        gradeText.text = text;
    }
    public void HideText()
    {
        gradeText.enabled = false;
    }

    public void ShowText()
    {
        gradeText.enabled = true;
    }

    // returns gradeText.enabled
    public bool GetEnabled() 
    {
        return gradeText.enabled;
    }

}
