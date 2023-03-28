using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    // Time system
    // second in real time in game time interval 
    public const float secondsPerGameInterval = 1f;
    // minutes in game time interval
    public const int timeInterval = 5;
    public static bool CheckValueInRange(float value,float minValue, float maxValue)
    {
        if(value >= minValue && value <= maxValue)
            return true;
        
        return false;
    }
}
