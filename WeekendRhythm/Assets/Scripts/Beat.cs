using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beat : MonoBehaviour
{
    public BeatMap.Direction direction;

    public float TimeSinceLastBeat { get; private set; }

    public bool spawned;

    public Beat(BeatMap.Direction direction, float timeSinceLastBeat, bool spawned)
    {
        this.direction = direction;
        this.TimeSinceLastBeat = timeSinceLastBeat;
        this.spawned = spawned;
    }
}
