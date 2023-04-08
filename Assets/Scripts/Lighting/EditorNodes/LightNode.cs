using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class LightNode
{
    public LightingBrightness lightingBrightness;

    [HideInInspector] private LightNodeSO startNode;
    public LightNode()
    {
        lightingBrightness.color = new Color32(255, 255, 255, 255);
    }

    public LightNode(LightNodeSO startNode) : this()
    {
        this.startNode = startNode;
    }

    public int GetTimeInMinutes()
    {
        return lightingBrightness.hour * 60 + lightingBrightness.minute;
    }

    #region Editor Code
#if UNITY_EDITOR


    public void DrawLightNode(LightNodeTypeSO lightNodeType)
    {
        EditorGUILayout.Space(10);

        if (lightNodeType.isStepBlock)
        {
            DrawStepBlockTimeFields();
        }
        else if (lightNodeType.isStartBlock)
        {
            DrawBlockedTimeFields(0, 0);
        }
        else if (lightNodeType.isEndBlock)
        {
            DrawBlockedTimeFields(23, 59);
        }

        if (lightingBrightness.hour < 0) lightingBrightness.hour = 0;
        if (lightingBrightness.hour > 23) lightingBrightness.hour = 23;
        if (lightingBrightness.minute < 0) lightingBrightness.minute = 0;
        if (lightingBrightness.minute > 59) lightingBrightness.minute = 59;


        if (lightingBrightness.lightIntensity < 0) lightingBrightness.lightIntensity = 0;

        EditorGUILayout.LabelField("Lighting Intensity: ");

        if (!lightNodeType.isEndBlock)
        {
            lightingBrightness.lightIntensity = EditorGUILayout.FloatField(lightingBrightness.lightIntensity);
        }
        else
        {
            lightingBrightness.lightIntensity = startNode.lightNode.lightingBrightness.lightIntensity;

            EditorGUILayout.LabelField(lightingBrightness.lightIntensity.ToString());

        }

        EditorGUILayout.LabelField("Light Color: ");

        if (!lightNodeType.isEndBlock)
        {
            lightingBrightness.color = EditorGUILayout.ColorField(lightingBrightness.color);
        }
        else
        {
            lightingBrightness.color = startNode.lightNode.lightingBrightness.color;

            EditorGUILayout.ColorField(lightingBrightness.color);

        }
    }

    public void DrawVerificationNote(LightNodeSO childNode, LightNodeSO parentNode)
    {
        if (
            VerifyData(
            ConvertTimeToMinutes(childNode.lightNode.lightingBrightness.hour, childNode.lightNode.lightingBrightness.minute),
            ConvertTimeToMinutes(lightingBrightness.hour, lightingBrightness.minute),
            ConvertTimeToMinutes(parentNode.lightNode.lightingBrightness.hour, parentNode.lightNode.lightingBrightness.minute))
            || childNode.lightNodeType.isStartBlock
            || parentNode.lightNodeType.isEndBlock
            )
        {
            EditorGUILayout.HelpBox("Verification was successful! ", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("Time should be smaller than child node, and higher than parent node!!", MessageType.Warning);
        }

    }

    private int ConvertTimeToMinutes(int hour, int minute)
    {
        return hour * 60 + minute;
    }

    private bool VerifyData(int childNode, int actualNode, int parentNode)
    {
        int childFinalTime = childNode + Settings.timeInterval - childNode % Settings.timeInterval;
        int actualFinalTime = actualNode + Settings.timeInterval - actualNode % Settings.timeInterval;
        int parentFinalTime = parentNode + Settings.timeInterval - parentNode % Settings.timeInterval;

        if (childFinalTime > actualFinalTime && actualFinalTime > parentFinalTime)
        {
            return true;
        }

        return false;
    }
    private void DrawStepBlockTimeFields()
    {
        EditorGUILayout.LabelField("Hour: ");
        lightingBrightness.hour = EditorGUILayout.IntField(lightingBrightness.hour);
        EditorGUILayout.LabelField("Minute: ");
        lightingBrightness.minute = EditorGUILayout.IntField(lightingBrightness.minute);
    }

    private void DrawBlockedTimeFields(int hour, int minute)
    {
        lightingBrightness.hour = hour;
        lightingBrightness.minute = minute;

        EditorGUILayout.LabelField("Hour: ");
        EditorGUILayout.LabelField(lightingBrightness.hour.ToString());
        EditorGUILayout.LabelField("Minute: ");
        EditorGUILayout.LabelField(lightingBrightness.minute.ToString());

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
