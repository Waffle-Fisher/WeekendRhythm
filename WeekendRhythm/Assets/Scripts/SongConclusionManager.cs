using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongConclusionManager : MonoBehaviour
{
    [SerializeField]
    private GameObject scmObject;

    public void ConcludeSong()
    {
        PauseGameController.Instance.Pause();
        scmObject.SetActive(true);
        Debug.Log("Finished Song");
    }
}
