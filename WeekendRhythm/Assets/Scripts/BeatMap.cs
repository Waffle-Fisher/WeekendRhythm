using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BeatMap : MonoBehaviour
{
    public static BeatMap Instance { get; private set; }
    public enum Direction {Up, Down, Left, Right, None};
    
    [System.Serializable]
    public struct Beat{
        [Min(0)]
        [SerializeField]
        public float timeOccursAt { get; private set; };
        
        [SerializeField]
        public Direction direction { get; private set;};
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
    private float timeSinceStart = 0f;
    float distance;
    int curBeat = 0;
    
    void Awake()
    {
        if(Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        InitializeBeatsAndBeatObjects();
    }

    void Update()
    {
        timeSinceStart += Time.deltaTime;
        if(timeSinceStart >= beats[curBeat].timeOccursAt - timeOffset)
        {
            //MoveBeat(beatObjects[curBeat])
        }
        //MoveBeat(beatObjects[0]);
        //for next 5(?) beats
        //check if they should spawn in
        //if they should spawn them and start moving them to other side of screen
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
        distance = spawnPos.x - endPos.x;
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
    }
}
