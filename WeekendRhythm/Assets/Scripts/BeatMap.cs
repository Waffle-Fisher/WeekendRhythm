using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BeatMap : MonoBehaviour
{
    public enum Direction {Up, Down, Left, Right, None};
    
    [System.Serializable]
    public struct Beat{
        [Min(0)]
        [SerializeField]
        float timeOccursAt;
        
        [SerializeField]
        public Direction direction;
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
    [SerializeField][Min(0)] 
    private float speed;

    
    void Start()
    {
        if (beats == null){ Debug.LogError("No Beats"); return;}
        beatObjects = new List<Transform>();
        GetComponentsInChildren<Transform>(true, beatObjects);
        beatObjects.RemoveAt(0);
        if(beats.Count > beatObjects.Count){ Debug.LogError("Not enough beats"); return;}
        foreach(Transform t in beatObjects){ t.gameObject.SetActive(false);}
        beatObjects.RemoveRange(beats.Count,beatObjects.Count - beats.Count);

        for(int i = 0; i < beats.Count; i++)
        {
            // adjust beatObjects pos based on time
            beatObjects[i].position = new Vector3(2 * i,0,0);
            SpriteRenderer s = beatObjects[i].GetComponent<SpriteRenderer>();
            //beatObjects[i].gameObject.SetActive(false);
            ChangeSprite(s, beats[i].direction);
        }
    }

    // Update is called once per frame
    void Update()
    {

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
}
