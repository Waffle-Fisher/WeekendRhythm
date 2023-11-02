using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Settings", menuName = "Assets/Settings", order = 1)]
public class SettingsScriptableObject : ScriptableObject
{
    [Range(0f,1f)]
    public float MasterVolumePercent = 1f;
    [Range(0f,1f)]
    public float MusicVolumePercent = 1f;
    [Range(0f,1f)]
    public float SFXVolumePercent = 1f;
}
