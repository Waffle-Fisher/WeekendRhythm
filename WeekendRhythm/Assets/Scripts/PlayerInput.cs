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
    float marginError = 0.1f;
    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Start()
    {

    }

    void Update()
    {
        BeatMap.Direction inDir = getInput();
        
    }

    BeatMap.Direction getInput()
    {
        Vector2 v2 = input.ReadValue<Vector2>();
        if(v2.y >= 1) { return BeatMap.Direction.Up; }
        else if(v2.y <= -1) { return BeatMap.Direction.Down; }
        else if(v2.x >= 1) { return BeatMap.Direction.Left; }
        else { return BeatMap.Direction.Right;}
    }

    BeatMap.Direction getCurrentButtonDirection()
    {
        return BeatMap.Direction.Left;
    }

}
