using System;
using UnityEngine;

[DisallowMultipleComponent]
public class TimeChangeEvent : MonoBehaviour
{
    public event Action<TimeChangeEvent, TimeChangeArg> OnTimeChange;

    public void CallTimeChangeEvent(Biome currentBiome, int gameDay, int gameHour, int gameMinute)
    {
        OnTimeChange?.Invoke(this, new TimeChangeArg()
        {
            currentBiome = currentBiome,
            gameDay = gameDay,
            gameHour = gameHour,
            gameMinute = gameMinute
        });
    }
}

public class TimeChangeArg : EventArgs
{
    public Biome currentBiome;
    public int gameDay;
    public int gameHour;
    public int gameMinute;
}