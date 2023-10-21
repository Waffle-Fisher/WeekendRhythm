using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BeatMap : MonoBehaviour
{
    public static BeatMap Instance { get; private set; }
    [Serializable]
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
    private List<Beat> beats = new();
    [SerializeField]
    private GameObject beatObject;
    
    [Header("Settings")]
    [SerializeField]
    private Vector3 spawnPos = new Vector3(6, 0, 0);
    [SerializeField]
    private Vector3 endPos = new Vector3(-6, 0, 0);
    
    [SerializeField][Min(0)][Tooltip("How long it should take the beat to move from spawn to beatdetector")]
    private float travelTime;
    [SerializeField][Min(0)][Tooltip("Delay between game start and when the song starts playing")]
    private float startDelay;

    [SerializeField]
    private bool randomizeBeatMap = false;
    [SerializeField]
    [Min(0.001f)]
    private float beatSpaceMin;
    [SerializeField]
    [Min(0.001f)]
    private float beatSpaceMax;

    private List<GameObject> beatObjects;
    private Vector3 detectorPos = new Vector3(-4, 0, 0);
    private int CurrentBeatIndex = 0;
    private int LatestBeatInd = 0;
    private float LatestBeatTime = 0f;
    private float movementSpeed = 0f; // units per second

    public enum Direction { Up, Down, Left, Right, None };

    public float TimeSinceStart { get; private set; } = 0f;
    public Beat CurrentBeat { get; private set; }

    void Awake()
    {
        if(Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        detectorPos = PlayerInput.Instance.transform.position;
        if(beatSpaceMin > beatSpaceMax) { Debug.LogError("beatSpaceMin is greater than beatSpaceMax"); }
        if(travelTime == 0) { Debug.LogError("Travel Time is 0. Leads to division by 0"); }
        movementSpeed = (spawnPos.x - detectorPos.x) / travelTime;
        if (randomizeBeatMap)
        {
            RandomizeBeatMapping();
        }
        InitializeBeatObjectPool();
    }

    void FixedUpdate()
    {
        TimeSinceStart += Time.fixedDeltaTime;

        while (LatestBeatInd < beats.Count && LatestBeatTime <= TimeSinceStart - beats[LatestBeatInd].TimeSinceLastBeat)
        {
            LatestBeatTime += beats[LatestBeatInd].TimeSinceLastBeat;
            LatestBeatInd++;
        }
        float totalBeatTimes = 0f;
        for (int i = CurrentBeatIndex; i < LatestBeatInd; i++)
        {
            if (!beatObjects[i].activeSelf)
            {
                float beatPosX = spawnPos.x + (totalBeatTimes + beats[i].TimeSinceLastBeat) * movementSpeed;
                Vector3 beatPos = new(beatPosX, spawnPos.y, spawnPos.z);
                beatObjects[i].transform.position = beatPos;
            }
            if (!beatObjects[i].activeSelf) { beatObjects[i].SetActive(true); }
            MoveBeat(beatObjects[i]);
            totalBeatTimes += beats[i].TimeSinceLastBeat;
        }

        //Debug.Log("Time: " + TimeSinceStart + "\n");
        //Debug.Log("CurBeat: " + CurrentBeatIndex + "\n" + "LatestBeat: " + LatestBeat + "\n");
    }

    public void IncrementCurrentBeat()
    {
        beatObjects[CurrentBeatIndex].SetActive(false);
        if (CurrentBeatIndex + 1 >= beatObjects.Count) { return; }
        CurrentBeatIndex++;
        BeatCountUpdater.Instance.UpdateText(CurrentBeatIndex);
        CurrentBeat = beats[CurrentBeatIndex];
    }

    public float GetDistanceDifference()
    {
        float delta = beatObjects[CurrentBeatIndex].transform.position.x - detectorPos.x;
        //Debug.Log("Current Beat Pos: " + beatObjects[CurrentBeatIndex].transform.position);
        //Debug.Log("Difference between detector and beat " + CurrentBeatIndex + ": " + delta);
        return delta;
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

    private void RandomizeBeatMapping()
    {
        for (int i = 1; i < beats.Count; i++)
        {
            beats[i] = new(UnityEngine.Random.Range(beatSpaceMin, beatSpaceMax), (Direction)UnityEngine.Random.Range(0, 4));
        }
    }

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
            Vector3 beatPos = new(spawnPos.x + (totalBeatTimes + beats[i].TimeSinceLastBeat) * (spawnPos.x - detectorPos.x) / travelTime, spawnPos.y, spawnPos.z);
            UnityEditor.Handles.DrawSolidDisc(beatPos, Vector3.back, 0.1f);
            totalBeatTimes += beats[i].TimeSinceLastBeat;
        }
    }
}
