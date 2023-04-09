using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Rendering;

[Serializable]
[CreateAssetMenu(fileName = "CurveDetailsSO_", menuName = "Scriptable Objects/Details/CurveDetailsSO")]
public class CurveDetailsSO : ScriptableObject
{
    public ParticleSystem.MinMaxCurve minMaxCurve;
    public float GetValueFromCurve(float time)
    {
        float value = minMaxCurve.Evaluate(time);

        return value;
    }
}
