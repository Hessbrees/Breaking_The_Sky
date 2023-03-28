using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactorsManager : MonoBehaviour
{
    [HideInInspector] public float temperatureFactor;
    [HideInInspector] public float polutionFactor;
    [HideInInspector] public float radiationFactor;

    public Action<float> temperatureChangeEvent;
    public Action<float> pulutionChangeEvent;
    public Action<float> radiationChangeEvent;



}
