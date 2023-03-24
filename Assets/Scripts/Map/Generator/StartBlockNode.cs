using System;
using UnityEngine;

public class StartBlockNode
{
    public string startBlockName = "";


    // Chance to spawn every time step (for exaple: 1 clock tick => 5 min in game, then 288 tick => 1 day in game) 
    public int baseFactor;
    // Spawning settings
    public float minTemperature;
    public float maxTemperature;
    public float minPolution;
    public float maxPolution;
    public float minRadiation;
    public float maxRadiation;


    public float GetSpawnChance(float currentTemperature, float currentPolution, float currentRadiation)
    {
        return baseFactor/288 *
            factorProbalibityCalculation(minTemperature, maxTemperature, currentTemperature) *
            factorProbalibityCalculation(minPolution, maxPolution, currentPolution) *
            factorProbalibityCalculation(minRadiation, maxRadiation, currentRadiation);
    }
    private float factorProbalibityCalculation(float minValue, float maxValue, float currentValue)
    {
        // find value between current value and middle value 
        float currentPoints = MathF.Abs((maxValue + minValue)/2 - MathF.Abs(currentValue));

        // find maximum value between minimum and middle value
        float maxPoints = (maxValue - minValue)/2;

        return (maxPoints - currentPoints) / maxPoints;
    }
}
