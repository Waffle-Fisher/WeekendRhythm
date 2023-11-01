using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;

public class BeatMapHandler : MonoBehaviour
{
    public static BeatMapHandler Instance { get; private set; }
    [Serializable]
    public struct Beat
    {
        [Min(0f)]
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
    [Header("BeatMap")]
    //[SerializeField]
    //private bool randomizeBeatMap = false;
    [SerializeField]
    private BeatMapScriptableObject levelBeatMapSO;
    [SerializeField]
    private List<Beat> beats = new();
    [SerializeField]
    private GameObject beatObject;
    [Space(8)]
    [Header("Settings")]
    [SerializeField]
    private Vector3 spawnPos = new Vector3(6, 0, 0);
    [SerializeField]
    private Vector3 endPos = new Vector3(-6, 0, 0);
    
    [SerializeField][Min(0)][Tooltip("How long it should take the beat to move from spawn to beatdetector")]
    private float travelTime;
    [SerializeField][Min(0)][Tooltip("Delay between game start and when the song starts playing")]
    private float startDelay = 0f;
    [SerializeField][Min(0)][Tooltip("Delay between game start and when the song starts playing")]
    private float finishDelayBuffer = 0f;

    
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
    private SongConclusionManager scm;
    private BeatMapScriptableObject randomizedBMSO;

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
        beats = levelBeatMapSO.beatMap;
        scm = GetComponent<SongConclusionManager>();
        detectorPos = PlayerInput.Instance.transform.position;
        if(beatSpaceMin > beatSpaceMax) { Debug.LogError("beatSpaceMin is greater than beatSpaceMax"); }
        if(travelTime == 0) { Debug.LogError("Travel Time is 0. Leads to division by 0"); }
        movementSpeed = (spawnPos.x - detectorPos.x) / travelTime;
        //if (randomizeBeatMap)
        //{
        //    beats = randomizedBMSO.beatMap;
        //    RandomizeBeatMapping();
        //}
        InitializeBeatObjects();
        StartCoroutine(JukeboxController.Instance.PlaySong(startDelay));
    }

    void FixedUpdate()
    {
        TimeSinceStart += Time.fixedDeltaTime;

        while (LatestBeatInd < beats.Count && LatestBeatTime <= TimeSinceStart - startDelay - beats[LatestBeatInd].TimeSinceLastBeat)
        {
            LatestBeatTime += beats[LatestBeatInd].TimeSinceLastBeat;
            LatestBeatInd++;
        }
        float totalBeatTimes = 0f;
        for (int i = CurrentBeatIndex; i < LatestBeatInd; i++)
        {
            //Debug.Log("CurrentBeatIndex: " + CurrentBeatIndex);
            //Debug.Log("LatestBeatIndex: " + LatestBeatInd);
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
        CurrentBeatIndex++;
        BeatCountUpdater.Instance.UpdateText(CurrentBeatIndex);
        if (CurrentBeatIndex >= beatObjects.Count) {
            Debug.Log("Processing Song Conlcusion");
            StartCoroutine(ProcessSongConclusion());
        }
        else
        {
            CurrentBeat = beats[CurrentBeatIndex];
        }
    }
    public float GetDistanceDifference()
    {
        if (CurrentBeatIndex >= beatObjects.Count) { return 999f; }
        float delta = beatObjects[CurrentBeatIndex].transform.position.x - detectorPos.x;
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
    private void InitializeBeatObjects()
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
    private IEnumerator ProcessSongConclusion()
    {
        float timeWait = JukeboxController.Instance.AudioSource.clip.length - TimeSinceStart + startDelay + finishDelayBuffer;
        yield return new WaitForSeconds(timeWait);
        scm.ConcludeSong();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(spawnPos, Vector3.one * 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(endPos, Vector3.one * 0.1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(Vector3.zero, Vector3.one * 0.1f);

        float totalBeatTimes = 0f;
        for(int i = 0; i < levelBeatMapSO.beatMap.Count; i++)
        {
            Handles.color = (i % 2 == 0) ? Color.blue : Color.yellow;
            Vector3 beatPos = new(spawnPos.x + (totalBeatTimes + levelBeatMapSO.beatMap[i].TimeSinceLastBeat) * (spawnPos.x - detectorPos.x) / travelTime, spawnPos.y, spawnPos.z);
            Handles.DrawSolidDisc(beatPos, Vector3.back, 0.1f);
            totalBeatTimes += levelBeatMapSO.beatMap[i].TimeSinceLastBeat;
        }
    }
#endif
}
