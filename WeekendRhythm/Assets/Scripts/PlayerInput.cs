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
    InputAction escape;
    [SerializeField]
    [Tooltip("The latest timeDif a beat can score a great")]
    float greatMargin = 0.5f;
    [SerializeField]
    [Tooltip("How far, in seconds, a beat can be detected. Must be greater than greatMargin")]
    float inputDistanceRange = 2f;
    [SerializeField]
    [Tooltip("How long the grade should stay on the screen")]
    float GradeDisplayLength = 1f;
    [SerializeField]
    [Tooltip("How long the grade should stay on the screen")]
    [Min(0)]
    int AudioClipIndex = 0;


    float GradeDisplayTimer = 0f;

    BeatGradeUpdater bguInstance;
    private PlayerSFX pSFX;
    void OnEnable()
    {
        input.Enable();
        escape.Enable();
    }

    void OnDisable()
    {
        input.Disable();
        escape.Disable();
    }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        if (inputDistanceRange < greatMargin) { Debug.LogError("inputTimeRange is smaller than greatMargin"); }
        bguInstance = BeatGradeUpdater.Instance;
        pSFX = GetComponent<PlayerSFX>();
    }

    void Update()
    {
        RemoveGradeText();
        if (escape.WasPressedThisFrame())
        {
            //ProcessEscape();
        }
        if (input.WasPressedThisFrame()) {
            ProcessGrade();
        }
    }

    private void ProcessGrade()
    {
        pSFX.PlayAudioClip(AudioClipIndex);
        float distanceDifference = BeatMapHandler.Instance.GetDistanceDifference();
        if (distanceDifference > inputDistanceRange) { return; }
        ScoreGradeHit(distanceDifference);
        BeatMapHandler.Instance.IncrementCurrentBeat();
    }

    private void RemoveGradeText()
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
    }

    private void ScoreGradeHit(float distDif)
    {
        Debug.Log("Distance Difference:" + distDif);
        if(bguInstance.GetEnabled()){ bguInstance.HideText(); }
        if (distDif > inputDistanceRange) { bguInstance.UpdateText("Miss"); }
        else if(GetInput() != BeatMapHandler.Instance.CurrentBeat.direction) { bguInstance.UpdateText("Wrong"); }
        else if (distDif < greatMargin) { bguInstance.UpdateText("Great");}
        else { bguInstance.UpdateText("Nice"); }
        bguInstance.ShowText();
    }

    BeatMapHandler.Direction GetInput()
    {
        Vector2 v2 = input.ReadValue<Vector2>();
        if (v2.y >= 1) { return BeatMapHandler.Direction.Up; }
        else if (v2.y <= -1) { return BeatMapHandler.Direction.Down; }
        else if (v2.x <= -1) { return BeatMapHandler.Direction.Left; }
        else { return BeatMapHandler.Direction.Right; }
    }

    public void HideZeroGradeDisplayTimer()
    {
        bguInstance.HideText();
        GradeDisplayTimer = 0f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector2(transform.position.x + inputDistanceRange, 10), new Vector2(transform.position.x + inputDistanceRange, -10));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(transform.position.x + greatMargin, 10), new Vector2(transform.position.x + greatMargin, -10));
    }

}
