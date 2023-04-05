using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class StartBlockNode
{
    // Chance to spawn every time step (for exaple: 1 clock tick => 5 min in game, then 288 tick => 1 day in game) 
    public int baseFactor;
    // Spawning settings
    public float minTemperature;
    public float maxTemperature;
    public float minPolution;
    public float maxPolution;
    public float minRadiation;
    public float maxRadiation;
    public GameObject startBlockPrefab;

    public float GetSpawnChance(Factors factors)
    {
        return baseFactor / 288 *
            factorProbalibityCalculation(minTemperature, maxTemperature, factors.temperature) *
            factorProbalibityCalculation(minPolution, maxPolution, factors.polution) *
            factorProbalibityCalculation(minRadiation, maxRadiation, factors.radiation);
    }
    private float factorProbalibityCalculation(float minValue, float maxValue, float currentValue)
    {
        if (minValue == maxValue && currentValue == minValue) return 1f;

        if (minValue == maxValue && currentValue != minValue) return 0;

        // find value between current value and middle value 
        float currentPoints = MathF.Abs((maxValue + minValue) / 2 - MathF.Abs(currentValue));

        // find maximum value between minimum and middle value
        float maxPoints = (maxValue - minValue) / 2;

        return (maxPoints - currentPoints) / maxPoints;
    }

    #region Editor Code
#if UNITY_EDITOR


    public void DrawStartBlock()
    {
        EditorGUILayout.Space(30);
        EditorGUILayout.LabelField("Prefab: ");
        startBlockPrefab = (GameObject)EditorGUILayout.ObjectField(startBlockPrefab, typeof(GameObject),true);

        if(startBlockPrefab != null)
        {
            if(startBlockPrefab.TryGetComponent(out Image image))
            {
                EditorGUI.DrawPreviewTexture(new Rect(160, 25, 60, 60),image.mainTexture);
            }
            else if(startBlockPrefab.TryGetComponent(out SpriteRenderer sprite))
            {
                EditorGUI.DrawPreviewTexture(new Rect(160, 25, 60, 60), sprite.sprite.texture);
            }
        }

        EditorGUILayout.LabelField("Base spawn amount every day: ");
        baseFactor = EditorGUILayout.IntField(baseFactor);

        EditorGUILayout.LabelField("Temperature spawn factor: ");
        minTemperature = EditorGUILayout.Slider(minTemperature, 0, maxTemperature);
        maxTemperature = EditorGUILayout.Slider(maxTemperature, minTemperature, 100);
        EditorGUILayout.MinMaxSlider(ref minTemperature, ref maxTemperature, 0, 100);

        EditorGUILayout.LabelField("Polution spawn factor: ");
        minPolution = EditorGUILayout.Slider(minPolution, 0, maxPolution);
        maxPolution = EditorGUILayout.Slider(maxPolution, minRadiation, 100);
        EditorGUILayout.MinMaxSlider(ref minPolution, ref maxPolution, 0, 100);

        EditorGUILayout.LabelField("Radiation spawn factor: ");
        minRadiation = EditorGUILayout.Slider(minRadiation, 0, maxRadiation);
        maxRadiation = EditorGUILayout.Slider(maxRadiation, minRadiation, 100);
        EditorGUILayout.MinMaxSlider(ref minRadiation, ref maxRadiation, 0, 100);

    }

#endif
    #endregion Editor Code
}
