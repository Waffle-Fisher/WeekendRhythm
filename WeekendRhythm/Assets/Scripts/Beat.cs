using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beat : MonoBehaviour
{
    public enum DirectionOptions { Up, Down, Left, Right, None };

    public float TimeSinceLastBeat { get; private set; }

    public DirectionOptions Direction { get; private set; }

    public bool spawned;
    public Beat(float time, DirectionOptions dir)
    {
        TimeSinceLastBeat = time;
        Direction = dir;
        spawned = false;
    }

    public void SetSpawned(bool b) { spawned = b; }
}
