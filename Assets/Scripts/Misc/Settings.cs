using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Settings
{
    // Time system
    // second in real time in game time interval 
    public const float secondsPerGameInterval = 0.05f;
    // minutes in game time interval
    public const int timeInterval = 5;

    #region PLAYER ANIMATOR PARAMETERS

    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");
    public static int isMovingFront = Animator.StringToHash("isMovingFront");
    public static int isMovingBack = Animator.StringToHash("isMovingBack");
    public static int isMovingLeft = Animator.StringToHash("isMovingLeft");
    public static int isMovingRight = Animator.StringToHash("isMovingRight");
    public static float baseSpeedForPlayerAnimations = 2f;

    #endregion


    #region FACTORS

    public static float minimumTemperaturePoints = 0;
    public static float maximumTemperaturePoints = 100;
    public static float minimumPolutionPoints = 0;
    public static float maximumPolutionPoints = 100;
    public static float minimumRadiationPoints = 0;
    public static float maximumRadiationPoints = 100;

    #endregion
    // check if value is in range
    public static bool CheckValueInRange(float value,float minValue, float maxValue)
    {
        if(value >= minValue && value <= maxValue)
            return true;
        
        return false;
    }


}

