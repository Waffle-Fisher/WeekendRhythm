using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    BeatMap bm = BeatMap.Instance;
    [SerializeField] 
    InputAction input;
    [SerializeField]
    float marginError = 0.5f;
    float inputTimeRange = 1f;
    BeatMap.Direction inputDir;
    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        if(!input.WasPressedThisFrame()) { return; }
        Debug.Log("Input has been pressed");
        float timeDifferent = BeatMap.Instance.CurrentBeat.timeOccursAt - BeatMap.Instance.TimeSinceStart;
        if(timeDifferent > inputTimeRange) { return;}
        inputDir = GetInput();
        if(timeDifferent > marginError) { Debug.Log("Miss"); }
        else { Debug.Log("Nice"); }
        BeatMap.Instance.IncrementCurrentBeat();
    }

    BeatMap.Direction GetInput()
    {
        Vector2 v2 = input.ReadValue<Vector2>();
        if (v2.y >= 1) { return BeatMap.Direction.Up; }
        else if (v2.y <= -1) { return BeatMap.Direction.Down; }
        else if (v2.x >= 1) { return BeatMap.Direction.Left; }
        else { return BeatMap.Direction.Right; }
    }

}
