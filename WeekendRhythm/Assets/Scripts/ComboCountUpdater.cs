using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComboCountUpdater : MonoBehaviour
{
    public static ComboCountUpdater Instance { get; private set; }
    TextMeshProUGUI countText;
    public int Combo {get; private set; } = 0;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        countText = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateText()
    {
        countText.text = string.Format("Combo: {0:000}", Combo);
    }

    public void IncrementCombo()
    {
        Combo++;
    }

    public void ResetCombo()
    {
        Combo = 0;
    }

}
