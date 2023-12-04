using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSystem : MonoBehaviour
{
    public static PlayerInputSystem Instance { get; private set; }

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

    private PlayerInput playerInput;
    float GradeDisplayTimer = 0f;
    BeatGradeUpdater bguInstance;
    private PlayerSFX pSFX;
    private PauseGameController pauseGC;
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
        if (inputDistanceRange < greatMargin) { Debug.LogError("inputTimeRange is smaller than greatMargin"); }
        
        pSFX = GetComponent<PlayerSFX>();
        pauseGC = PauseGameController.Instance;
        playerInput = GetComponent<PlayerInput>();

        Controls controls = new Controls();
        controls.Enable();
        controls.Actions.Left.performed += PressedLeft;
        controls.Actions.Right.performed += PressedRight;
        controls.Actions.Down.performed += PressedDown;
        controls.Actions.Up.performed += PressedUp;
        controls.Actions.Escape.performed += Pause;
    }

    void Start()
    {
        bguInstance = BeatGradeUpdater.Instance;
    }
    void Update()
    {
        RemoveGradeText();
    }

    private void PressedLeft(InputAction.CallbackContext context)
    {
        if(!context.performed) { return; }
        ProcessGrade(BeatMapHandler.Direction.Left);
    }
    private void PressedRight(InputAction.CallbackContext context)
    {
        if(!context.performed) { return; }
        ProcessGrade(BeatMapHandler.Direction.Right);
    }
    private void PressedDown(InputAction.CallbackContext context)
    {
        if(!context.performed) { return; }
        ProcessGrade(BeatMapHandler.Direction.Down);
    }
    private void PressedUp(InputAction.CallbackContext context)
    {
        if(!context.performed) { return; }
        ProcessGrade(BeatMapHandler.Direction.Up);
    }
    private void Pause(InputAction.CallbackContext context)
    {
        if(!context.performed) { return; }
        if(!pauseGC.IsPaused)
            {
                pauseGC.Pause();
                pauseGC.ToggleSettingsMenu();
            }
            else {
                pauseGC.Resume();
                pauseGC.ToggleSettingsMenu();
            }
    }
    private void ProcessGrade(BeatMapHandler.Direction dir)
    {
        pSFX.PlayAudioClip(AudioClipIndex);
        float distanceDifference = BeatMapHandler.Instance.GetDistanceDifference();
        if (distanceDifference > inputDistanceRange) { return; }
        ScoreGradeHit(distanceDifference, dir);
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

    private void ScoreGradeHit(float distDif, BeatMapHandler.Direction dir)
    {
        Debug.Log("Distance Difference:" + distDif);
        if(bguInstance.GetEnabled()){ bguInstance.HideText(); }
        if (distDif > inputDistanceRange) { 
            ComboCountUpdater.Instance.ResetCombo();
            bguInstance.UpdateText("Miss");
        } else if(dir != BeatMapHandler.Instance.CurrentBeat.direction) { 
            ComboCountUpdater.Instance.ResetCombo();
            bguInstance.UpdateText("Wrong"); 
        } else if (distDif < greatMargin) { 
            bguInstance.UpdateText("Great");
            ComboCountUpdater.Instance.IncrementCombo();
            ScoreManager.Instance.AwardPoints(1);
        } else { 
            bguInstance.UpdateText("Nice");
            ComboCountUpdater.Instance.IncrementCombo();
            ScoreManager.Instance.AwardPoints(0);
        } bguInstance.ShowText();
    }

    // BeatMapHandler.Direction GetInput()
    // {
    //     Vector2 v2 = input.ReadValue<Vector2>();
    //     if (v2.y >= 1) { return BeatMapHandler.Direction.Up; }
    //     else if (v2.y <= -1) { return BeatMapHandler.Direction.Down; }
    //     else if (v2.x <= -1) { return BeatMapHandler.Direction.Left; }
    //     else { return BeatMapHandler.Direction.Right; }
    // }

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
