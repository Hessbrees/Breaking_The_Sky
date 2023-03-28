using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TimeChangeEvent))]
[DisallowMultipleComponent]
public class TimeManager : MonoBehaviour
{
    [HideInInspector] public TimeChangeEvent timeChangeEvent;

    private Biome currentBiome;
    
    private int gameDay = 1;
    private int gameHour = 6;
    public int gameMinute = 0;

    private bool gamePaused = false;

    private float gameTick = 0;

    private void Awake()
    {
        timeChangeEvent = GetComponent<TimeChangeEvent>();
    }
    private void Update()
    {
        if(!gamePaused)
        {
            GameTick();
        }
    }

    private void GameTick()
    {
        gameTick += Time.deltaTime;
        
        if(gameTick> Settings.secondsPerGameInterval)
        {
            gameTick -= Settings.secondsPerGameInterval;

            UpdateGameMinute();
        }
    }

    private void UpdateGameMinute()
    {
        gameMinute += Settings.timeInterval;
        
        if(gameMinute > 59)
        {
            gameMinute = 0;
            gameHour++;

            if(gameHour > 23)
            {
                gameHour = 0;
                gameDay++;

            }

        }

        timeChangeEvent.CallTimeChangeEvent(currentBiome, gameDay, gameHour, gameMinute);
    }

}
