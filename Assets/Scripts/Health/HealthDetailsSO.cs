using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "HealthDetails_", menuName = "Scriptable Objects/Health/HealthDetails")]
public class HealthDetailsSO : ScriptableObject
{
    public int maximumHealth;
    public int startingHealth;
    public int defence;
}
