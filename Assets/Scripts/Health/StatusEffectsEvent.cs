using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectsEvent : MonoBehaviour
{
    public event Action<StatusEffectsEvent, StatusEffectsArgs> OnStatusEffects;

    public void CallStatusEffectsEvent(float damageReduction, float healReduction, float damageOverTime, float movementReduction,float damageTaken)
    {
        OnStatusEffects?.Invoke(this, new StatusEffectsArgs()
        {
            damageReduction = damageReduction,
            healReduction = healReduction,
            damageOverTime = damageOverTime,
            movementReduction = movementReduction,
            damageTaken = damageTaken
        });
    }
}

public class StatusEffectsArgs : EventArgs
{
    public float damageReduction ;
    public float healReduction ;
    public float damageOverTime ;
    public float movementReduction ;
    public float damageTaken ;
}