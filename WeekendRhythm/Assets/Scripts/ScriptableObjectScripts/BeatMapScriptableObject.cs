using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BeatMap", menuName = "Assets/BeatMaps", order = 1)]
public class BeatMapScriptableObject : ScriptableObject
{
    public List<BeatMapHandler.Beat> beatMap = new();
}
