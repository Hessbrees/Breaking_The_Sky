using System;
using UnityEditor;
using UnityEngine;
[Serializable]
public class ConnectBlockNode
{
    // Minimal points needed to change state to step block
    // Get one every tick when factors are in requirements range
    public int connectBlockPoints;

    // Factors setting needed to change state
    public float minTemperature;
    public float maxTemperature;
    public float minPolution;
    public float maxPolution;
    public float minRadiation;
    public float maxRadiation;

    public bool IsFactorsRequirementsAreMet(float currentTemperature, float currentPolution, float currentRadiation)
    {
        if (Settings.CheckValueInRange(currentTemperature, minTemperature, maxTemperature) &&
        Settings.CheckValueInRange(currentPolution, minPolution, maxPolution) &&
        Settings.CheckValueInRange(currentRadiation, minRadiation, maxRadiation))
        {
            return true;
        }

        return false;
    }
    #region Editor Code
#if UNITY_EDITOR

    public void DrawConnectBlock()
    {
        EditorGUILayout.Space(30);

        EditorGUILayout.LabelField("Requirements tick to change: ");
        connectBlockPoints = EditorGUILayout.IntField(connectBlockPoints);

        EditorGUILayout.LabelField("Temperature spawn factor: ");
        minTemperature = EditorGUILayout.Slider(minTemperature, 0, maxTemperature);
        maxTemperature = EditorGUILayout.Slider(maxTemperature, minTemperature, 100);
        EditorGUILayout.MinMaxSlider(ref minTemperature, ref maxTemperature, 0, 100);

        EditorGUILayout.LabelField("Polution spawn factor: ");
        minPolution = EditorGUILayout.Slider(minPolution, 0, maxPolution);
        maxPolution = EditorGUILayout.Slider(maxPolution, minPolution, 100);
        EditorGUILayout.MinMaxSlider(ref minPolution, ref maxPolution, 0, 100);

        EditorGUILayout.LabelField("Radiation spawn factor: ");
        minRadiation = EditorGUILayout.Slider(minRadiation, 0, maxRadiation);
        maxRadiation = EditorGUILayout.Slider(maxRadiation, minRadiation, 100);
        EditorGUILayout.MinMaxSlider(ref minRadiation, ref maxRadiation, 0, 100);

    }

#endif
    #endregion Editor Code
}
