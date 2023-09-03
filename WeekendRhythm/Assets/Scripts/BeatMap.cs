using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class BeatMap : MonoBehaviour
{
    enum Direction {Up, Down, Left, Right, None};
    
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
    [SerializeField] private List<Beat> beats;
    List<GameObject> beatObject;
    
    
    void Start()
    {
        if (beats == null){ Debug.LogError("No Beats"); return;}
        beatObject = new List<GameObject>();
        gameObject.GetComponentsInChildren<GameObject>(true, beatObject);
        if(beats.Count > beatObject.Count){ Debug.LogError("Not enough beats"); return;}
        beatObject.RemoveRange(beats.Count,beatObject.Count - beats.Count);

        for(int i = 0; i < beats.Count; i++)
        {
            // adjust beatObject pos based on time
            beatObject[i].transform.position = new Vector3(0,0,0);
            //changeSprite(beatObject[i].GetComponent<SpriteRenderer>, beats[i].direction);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void changeSprite(SpriteRenderer sr, Direction d)
    {
        if(d == Direction.Up)
        {
            sr.sprite = Up;
        }
        else if(d == Direction.Down)
        {
            sr.Sprite = Down;
        }
        else if(d == Direction.Left)   
        {
            sr.Sprite = Left;
        }
        else
        {
            sr.Sprite = Right;
        }
    }
}
