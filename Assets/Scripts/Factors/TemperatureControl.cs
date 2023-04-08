using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TemperatureControl : MonoBehaviour
{
    [Inject(Id = "Factors")]
    private FactorsManager factorsManager;

    [Inject(Id = "TimeManager")]
    private TimeChangeEvent timeChangeEvent;

    private void OnEnable()
    {
        timeChangeEvent.OnTimeChange += TryChangeTemperature_OnTimeChange;
    }
    private void OnDisable()
    {
        timeChangeEvent.OnTimeChange -= TryChangeTemperature_OnTimeChange;
    }

    private void TryChangeTemperature_OnTimeChange(TimeChangeEvent timeChangeEvent, TimeChangeArg timeChangeArg)
    {
        if (timeChangeArg.gameHour > 10 && timeChangeArg.gameHour < 16)
        {
            factorsManager.currentFactors.temperature += Random.Range(-1, 6);
        }
        else
        {
            factorsManager.currentFactors.temperature += Random.Range(-3, 1);
        }

        if (factorsManager.currentFactors.temperature > 100) factorsManager.currentFactors.temperature = 100;
        if (factorsManager.currentFactors.temperature < 0) factorsManager.currentFactors.temperature = 0;
    }
}
