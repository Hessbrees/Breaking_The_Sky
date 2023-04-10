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

    [SerializeField] private CurveDetailsSO temperatureCurveDetails;

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
        GetRandomTemperatureChange(timeChangeArg.gameHour);
    }

    private void GetRandomTemperatureChange(float hour)
    {
        factorsManager.currentFactors.temperature += (float)System.Math.Round(temperatureCurveDetails.GetValueFromCurve(hour / 24),2);

        factorsManager.currentFactors.temperature =
            HelperUtilities.LimitValueToTargetRange(Settings.minimumTemperaturePoints, Settings.maximumTemperaturePoints, factorsManager.currentFactors.temperature);
    }
}
