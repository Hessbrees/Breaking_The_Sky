using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LightNode
{
    public LightingBrightness lightingBrightness;

    #region Editor Code
#if UNITY_EDITOR


    public void DrawLightNode()
    {
        EditorGUILayout.Space(30);

        EditorGUILayout.LabelField("Hour: ");
        lightingBrightness.hour = EditorGUILayout.IntField(lightingBrightness.hour);
        EditorGUILayout.LabelField("Minute: ");
        lightingBrightness.minute = EditorGUILayout.IntField(lightingBrightness.minute);

        if(lightingBrightness.hour <0) lightingBrightness.hour = 0;
        if(lightingBrightness.hour >23) lightingBrightness.hour = 23;
        if(lightingBrightness.minute<0) lightingBrightness.minute = 0;
        if(lightingBrightness.minute>23) lightingBrightness.minute = 23;

        EditorGUILayout.LabelField("lighting Intensity: ");
        lightingBrightness.lightIntensity = EditorGUILayout.FloatField(lightingBrightness.lightIntensity);
        
        if(lightingBrightness.lightIntensity <0) lightingBrightness.lightIntensity = 0;

        EditorGUILayout.LabelField("Light Color: ");
        lightingBrightness.color = EditorGUILayout.ColorField(lightingBrightness.color);
    
    }

#endif
    #endregion Editor Code
}
[Serializable]
public struct LightingBrightness
{
    public int hour;
    public int minute;
    public float lightIntensity;
    public Color32 color;

}
