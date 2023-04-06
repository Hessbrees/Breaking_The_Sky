using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "lightingSchedule_", menuName = "Scriptable Objects/Lighting/LightingSchedule")]
public class LightingScheduleSO : ScriptableObject
{
    public List<LightingBrightness> lightingBrightnessesArray;
}

