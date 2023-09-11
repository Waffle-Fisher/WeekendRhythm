using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance { get; private set; }

    [SerializeField] 
    InputAction input;
    [SerializeField]
    [Tooltip("The latest timeDif a beat can score a great")]
    float greatMargin = 0.5f;
    [SerializeField]
    [Tooltip("How far, in seconds, a beat can be detected. Must be greater than greatMargin")]
    float inputTimeRange = 2f;
    [SerializeField]
    [Tooltip("How long the grade should stay on the screen")]
    float GradeDisplayLength = 1f;

    float GradeDisplayTimer = 0f;

    BeatGradeUpdater bguInstance;
    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        bguInstance = BeatGradeUpdater.Instance;
        if (inputTimeRange < greatMargin) { Debug.LogError("inputTimeRange is smaller than greatMargin"); }
    }

    void Update()
    {
        if (bguInstance.GetEnabled())
        {
            if (GradeDisplayTimer >= GradeDisplayLength)
            {
                HideZeroGradeDisplayTimer();
            }
            else
            {
                GradeDisplayTimer += Time.deltaTime;
            }
        }
        if (!input.WasPressedThisFrame()) { return; }
        float timeDifferent = BeatMap.Instance.GetTimeDifference();
        //Debug.Log("Time Difference: " + timeDifferent);
        if (timeDifferent > inputTimeRange) { return; }
        GradeHit(timeDifferent);
    }

    private void GradeHit(float timeDifferent)
    {
        Debug.Log("Time Difference:" + timeDifferent);
        if(bguInstance.GetEnabled()){ bguInstance.HideText(); }
        if (timeDifferent > inputTimeRange) { bguInstance.UpdateText("Miss"); }
        else if(GetInput() != BeatMap.Instance.CurrentBeat.direction) { bguInstance.UpdateText("Wrong"); }
        else if (timeDifferent < greatMargin) { bguInstance.UpdateText("Great");}
        else { bguInstance.UpdateText("Nice"); }
        bguInstance.ShowText();
        BeatMap.Instance.IncrementCurrentBeat();
    }

    BeatMap.Direction GetInput()
    {
        Vector2 v2 = input.ReadValue<Vector2>();
        if (v2.y >= 1) { return BeatMap.Direction.Up; }
        else if (v2.y <= -1) { return BeatMap.Direction.Down; }
        else if (v2.x <= -1) { return BeatMap.Direction.Left; }
        else { return BeatMap.Direction.Right; }
    }

    public void HideZeroGradeDisplayTimer()
    {
        bguInstance.HideText();
        GradeDisplayTimer = 0f;
    }
    
}
