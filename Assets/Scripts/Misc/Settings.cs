using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    #region BLOCK SETTINGS

    #endregion


    public static bool CheckValueInRange(float value,float minValue, float maxValue)
    {
        if(value >= minValue && value <= maxValue)
            return true;
        
        return false;
    }
}
