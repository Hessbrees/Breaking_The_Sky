using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities
{
    public static int GetTimeInMinutes(int hour, int minute)
    {
        return hour * 60 + minute;
    }
    public static float LimitValueToTargetRange(float minValue, float maxValue, float currentValue)
    {
        if(currentValue > maxValue) currentValue = maxValue;
        else if(currentValue < minValue) currentValue = minValue;

        return currentValue;
    }

    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.Log(fieldName + " is empty and must contain a value in object " + thisObject.name.ToString());
            return true;
        }
        return false;
    }

    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        if (enumerableObjectToCheck == null)
        {
            Debug.Log(fieldName + " is null in object " + thisObject.name.ToString());
            return true;
        }


        foreach (var item in enumerableObjectToCheck)
        {

            if (item == null)
            {
                Debug.Log(fieldName + " has null values in object " + thisObject.name.ToString());
                error = true;
            }
            else
            {
                count++;
            }
        }

        if (count == 0)
        {
            Debug.Log(fieldName + " has no values in object " + thisObject.name.ToString());
            error = true;
        }

        return error;
    }

    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMinimum, float minimumValueToCheck, string fieldNameMaximum, float maximumValueToCheck)
    {
        if(minimumValueToCheck > maximumValueToCheck)
        {
            Debug.Log(fieldNameMinimum + " is more than " + maximumValueToCheck + " in object " + thisObject);
            
            return true;
        }

        return false;
    }
}
