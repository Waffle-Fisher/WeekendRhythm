using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance {get; private set;}
    [SerializeField]
    [Tooltip("Smallest Score First, Highest Score last in index")]
    private List<int> PointValues = new();

    [SerializeField]
    [Tooltip("First value is the minimum combo score to reach, Second value is the amount of extra points awarded when you correctly hit a beat")]
    private List<Vector2Int> ComboValues = new();

    public int Score { get; private set;} = 0;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void AwardPoints(int index){
        Score += PointValues[index];
        Score += ComboBonus();
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI scoreText))
        {
            scoreText.text = String.Format("Score: {0:0000}",Score);
        }
    }

    private int ComboBonus()
    {
        if(ComboValues.Count == 1) {return ComboValues[0].y;}
        int c  = ComboCountUpdater.Instance.Combo;
        int comboValuesInd = 0;
        while(comboValuesInd+1 < ComboValues.Count && c  > ComboValues[comboValuesInd+1].x)
        {
            comboValuesInd++;
        }
        return ComboValues[comboValuesInd].y;
    }
}
