using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthEvent : MonoBehaviour
{
    public event Action<HealthEvent, HealthArg> OnHealth;

    public void CallHealthEvent(float percentHealth, float currentHealth, int maxHealth,int defence, bool isDead)
    {
        OnHealth?.Invoke(this, new HealthArg()
        {
            percentHealth = percentHealth,
            currentHealth = currentHealth,
            maxHealth = maxHealth,
            defence = defence,
            isDead = isDead
        });
    }
}
public class HealthArg: EventArgs
{
    public float percentHealth;
    public float currentHealth;
    public int maxHealth;
    public int defence;
    public bool isDead;
}
    
