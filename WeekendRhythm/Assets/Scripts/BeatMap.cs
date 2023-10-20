using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BeatMap : MonoBehaviour
{
    public static BeatMap Instance { get; private set; }
    public struct Beat
    {
        [Min(0)]
        [SerializeField]
        public float TimeSinceLastBeat;// { get; private set };

        [SerializeField]
        public Direction direction;// { get; private set};

        public Beat(float time, Direction dir)
        {
            TimeSinceLastBeat = time;
            direction = dir;
        }
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
    private List<Tuple<Beat,bool>> beats = new();
    [SerializeField]
    private GameObject beatObject;
    
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

    [SerializeField]
    private bool randomizeBeatMap = false;
    [SerializeField]
    [Min(0)]
    private float beatSpaceMin;
    [SerializeField]
    [Min(0)]
    private float beatSpaceMax;

    private List<GameObject> beatObjects;

    public enum Direction { Up, Down, Left, Right, None };

    public float TimeSinceStart { get; private set; } = 0f;
    public int CurrentBeatIndex { get; private set; } = 0;
    public Beat CurrentBeat { get; private set; }

    private int LatestBeat = 0;
    private float LatestBeatTime = 0f;
    private float movementSpeed = -1f;

    public void IncrementCurrentBeat()
    {
        beatObjects[CurrentBeatIndex].SetActive(false);
        if (CurrentBeatIndex + 1 >= beatObjects.Count) { return; }
        CurrentBeatIndex++;
        BeatCountUpdater.Instance.UpdateText(CurrentBeatIndex);
        CurrentBeat = beats[CurrentBeatIndex].Item1;
    }

    public float GetTimeDifference()
    {
        return CurrentBeat.TimeSinceLastBeat + timeOffset - TimeSinceStart;
    }

    void Awake()
    {
        if(Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        if(beatSpaceMin > beatSpaceMax) { Debug.LogError("beatSpaceMin is greater than beatSpaceMax"); }
        movementSpeed = (spawnPos.x - detectorPos.x) / timeOffset;
        //if(randomizeBeatMap)
        //{
        //    RandomizeBeatMapping();
        //}
        InitializeBeatObjectPool();
    }

    private void Update()
    {
        CurrentBeat = beats[CurrentBeatIndex].Item1;
    }

    void FixedUpdate()
    {
        TimeSinceStart += Time.fixedDeltaTime;

        while (LatestBeat < beats.Count && LatestBeatTime <= TimeSinceStart - beats[LatestBeat].Item1.TimeSinceLastBeat)
        {
            LatestBeatTime += beats[LatestBeat].Item1.TimeSinceLastBeat;
            LatestBeat++;
        }
        float totalBeatTimes = 0f;
        for (int i = CurrentBeatIndex; i < LatestBeat; i++)
        {
            Debug.Log("Beat " + i + " has spawned?: " + beats[i].Item2);
            if (!beats[i].Item2)
            {
                float beatPosX = spawnPos.x + (totalBeatTimes + beats[i].Item1.TimeSinceLastBeat) * movementSpeed;
                Vector3 beatPos = new(beatPosX, spawnPos.y, spawnPos.z);
                beatObjects[i].transform.position = beatPos;
            }
            if (!beatObjects[i].activeSelf) { beatObjects[i].SetActive(true); }
            Debug.Log("Beat " + i + " is now moving");
            MoveBeat(beatObjects[i]);
            totalBeatTimes += beats[i].Item1.TimeSinceLastBeat;
        }

        //Debug.Log("Time: " + TimeSinceStart + "\n");
        //Debug.Log("CurBeat: " + CurrentBeatIndex + "\n" + "LatestBeat: " + LatestBeat + "\n");
    }
    void MoveBeat(GameObject g)
    {
        g.transform.Translate(movementSpeed * Time.deltaTime * Vector2.left);
        if (g.transform.position.x <= detectorPos.x)
        {
            IncrementCurrentBeat();
            PlayerInput.Instance.HideZeroGradeDisplayTimer();
            BeatGradeUpdater.Instance.UpdateText("Miss");
            BeatGradeUpdater.Instance.ShowText();
        }
    }

    private void InitializeBeatObjectPool()
    {
        beatObjects = new List<GameObject>();
        for (int i = 0; i < beats.Count; i++)
        {
            beatObjects.Add(Instantiate(beatObject, transform));
            beatObjects[i].SetActive(false);
            ChangeSprite(beatObjects[i].GetComponent<SpriteRenderer>(), beats[i].Item1.direction);
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

    //private void RandomizeBeatMapping()
    //{
    //    for (int i = 1; i < beats.Count; i++)
    //    {
    //        beats[i] = new((Direction)UnityEngine.Random.Range(0, 4), beats[i - 1].TimeSinceLastBeat + UnityEngine.Random.Range(beatSpaceMin, beatSpaceMax), false);
    //    }
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(spawnPos, Vector3.one * 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(endPos, Vector3.one * 0.1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(Vector3.zero, Vector3.one * 0.1f);

        float totalBeatTimes = 0f;
        for(int i = 0; i < beats.Count; i++)
        {
            UnityEditor.Handles.color = (i % 2 == 0) ? Color.blue : Color.yellow;
            Vector3 beatPos = new(spawnPos.x + (totalBeatTimes + beats[i].Item1.TimeSinceLastBeat) * (spawnPos.x - detectorPos.x) / timeOffset, spawnPos.y, spawnPos.z);
            UnityEditor.Handles.DrawSolidDisc(beatPos, Vector3.back, 0.1f);
            totalBeatTimes += beats[i].Item1.TimeSinceLastBeat;
        }
    }
}
