using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "lightingSchedule_", menuName = "Scriptable Objects/Lighting/LightingSchedule")]
public class LightingScheduleSO : ScriptableObject
{
    public LightingBrightness defaultBrightness;

    public List<LightingBrightness> lightingBrightnessesArray;
}

[Serializable]
public class LightingBrightness
{
    public int hour =15;
    public float lightIntensity=15;
    public Color32 color =new Color32(255, 255, 255, 255);

}