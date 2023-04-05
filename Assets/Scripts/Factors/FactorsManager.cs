
using System;
using UnityEngine;

[DisallowMultipleComponent]
public class FactorsManager : MonoBehaviour
{
    public Factors currentFactors;
}

[Serializable]
public struct Factors
{
    public float temperature;
    public float polution;
    public float radiation;
}