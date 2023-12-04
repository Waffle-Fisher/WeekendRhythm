using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SongConclusionManager : MonoBehaviour
{
    [SerializeField]
    private GameObject scmObject;
    [SerializeField]
    private TextMeshProUGUI scoreText;

    public void ConcludeSong()
    {
        scmObject.SetActive(true);
        scoreText.text = String.Format("Score: {0:0000}",ScoreManager.Instance.Score);
        PauseGameController.Instance.Pause();
    }
}
