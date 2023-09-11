using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BeatMap : MonoBehaviour
{
    public static BeatMap Instance { get; private set; }
    public enum Direction { Up, Down, Left, Right, None };

    [System.Serializable]
    public struct Beat {
        [Min(0)]
        [SerializeField]
        public float timeOccursAt;// { get; private set };

        [SerializeField]
        public Direction direction;// { get; private set};
    }

    [Header("BeatTypes")]
    [SerializeField]
    Sprite Up;
    [SerializeField]
    Sprite Down;
    [SerializeField]
    Sprite Left;
    [SerializeField]
    Sprite Right;
    [Space(8)]
    [SerializeField]
    private List<Beat> beats;
    List<Transform> beatObjects;
    [Header("Settings")]
    [SerializeField]
    private Vector3 spawnPos = new Vector3(6, 0, 0);
    [SerializeField]
    private Vector3 endPos = new Vector3(-6, 0, 0);
    [SerializeField]
    private Vector3 detectorPos = new Vector3(-4, 0, 0);
    [SerializeField][Min(0)][Tooltip("How long it should take the beat to move from spawn to beatdetector")]
    private float timeOffset;
    [SerializeField][Min(0)][Tooltip("Delay between game start and when the song starts playing")]
    private float startDelay;

    public float TimeSinceStart { get; private set; } = 0f;
    public int CurrentBeatIndex { get; private set; } = 0;

    private int LatestBeat = 0;

    public Beat CurrentBeat { get; private set; }

    void Awake()
    {
        if(Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        InitializeBeatsAndBeatObjects();
    }

    private void Update()
    {
        CurrentBeat = beats[CurrentBeatIndex];
    }

    void FixedUpdate()
    {
        TimeSinceStart += Time.fixedDeltaTime;
        while (LatestBeat < beats.Count && beats[LatestBeat].timeOccursAt <= TimeSinceStart)
        {
            LatestBeat++;
        }
        for (int i = CurrentBeatIndex; i < LatestBeat; i++)
        {
            if (!beatObjects[i].gameObject.activeSelf) { beatObjects[i].gameObject.SetActive(true); }
            MoveBeat(beatObjects[i]);
        }
        //Debug.Log("Time: " + TimeSinceStart + "\n");
        //Debug.Log("CurBeat: " + CurrentBeatIndex + "\n" + "LatestBeat: " + LatestBeat + "\n");
    }

    private void InitializeBeatsAndBeatObjects()
    {
        if (beats == null) { Debug.LogError("No Beats"); return; }
        beatObjects = new List<Transform>();
        GetComponentsInChildren<Transform>(true, beatObjects);
        beatObjects.RemoveAt(0);
        foreach (Transform t in beatObjects) { t.gameObject.SetActive(false); }
        if (beats.Count > beatObjects.Count) { Debug.LogError("Not enough beats"); return; }
        beatObjects.RemoveRange(beats.Count, beatObjects.Count - beats.Count);
        for (int i = 0; i < beats.Count; i++)
        {
            ChangeSprite(beatObjects[i].GetComponent<SpriteRenderer>(), beats[i].direction);
        }
    }
    void ChangeSprite(SpriteRenderer sr, Direction d)
    {
        if(d == Direction.Up)
        {
            sr.sprite = Up;
        }
        else if(d == Direction.Down)
        {
            sr.sprite = Down;
        }
        else if(d == Direction.Left)   
        {
            sr.sprite = Left;
        }
        else
        {
            sr.sprite = Right;
        }
    }
    
    void MoveBeat(Transform t){
        float speed = (spawnPos.x - detectorPos.x) / timeOffset;
        t.Translate(speed * Time.deltaTime * Vector2.left);
        if (t.position.x <= detectorPos.x)
        {
            IncrementCurrentBeat();
            PlayerInput.Instance.HideZeroGradeDisplayTimer();
            BeatGradeUpdater.Instance.UpdateText("Miss");
            BeatGradeUpdater.Instance.ShowText();
        }
    }

    public void IncrementCurrentBeat()
    {
        beatObjects[CurrentBeatIndex].gameObject.SetActive(false);
        if(CurrentBeatIndex + 1 >= beatObjects.Count) { return; }
        CurrentBeatIndex++;
        BeatCountUpdater.Instance.UpdateText(CurrentBeatIndex);
        CurrentBeat = beats[CurrentBeatIndex];
    }

    public float GetTimeDifference()
    {
        return CurrentBeat.timeOccursAt + timeOffset - TimeSinceStart;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(spawnPos, Vector3.one * 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(endPos, Vector3.one * 0.1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(Vector3.zero, Vector3.one * 0.1f);
    }
}
