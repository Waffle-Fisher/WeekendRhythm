using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class BeatMap : MonoBehaviour
{
    [System.Serializable]
    public struct Beat{
        [Min(0)]
        [SerializeField]
        float timeOccursAt;
        enum Direction {Up, Down, Left, Right, None};
        [SerializeField]
        Direction direction;
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
            // change beatObject sprite
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
