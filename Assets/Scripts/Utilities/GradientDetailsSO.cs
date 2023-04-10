using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
[CreateAssetMenu(fileName = "GradientDetailsSO_", menuName = "Scriptable Objects/Details/GradientDetailsSO")]
public class GradientDetailsSO : ScriptableObject
{
    public Gradient gradient;

    public Color GetColorFromGradient(float time)
    {
        return gradient.Evaluate(time);
    }
}
