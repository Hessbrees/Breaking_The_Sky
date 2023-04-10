using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

public class PolutionControl : MonoBehaviour
{
    [SerializeField] private CurveDetailsSO polutionCurveDetails;

    [Inject(Id = "Factors")]
    private FactorsManager factorsManager;

    [Inject(Id = "TimeManager")]
    private TimeChangeEvent timeChangeEvent;

    private void OnEnable()
    {
        timeChangeEvent.OnTimeChange += TryChangePolution_OnTimeChange;
    }
    private void OnDisable()
    {
        timeChangeEvent.OnTimeChange -= TryChangePolution_OnTimeChange;
    }
    private void TryChangePolution_OnTimeChange(TimeChangeEvent timeChangeEvent, TimeChangeArg timeChangeArg)
    {
        int timeMinute = HelperUtilities.GetTimeInMinutes(timeChangeArg.gameHour, timeChangeArg.gameMinute);

        TryChangePolution(timeMinute);
    }

    private void TryChangePolution(int minute)
    {
        factorsManager.currentFactors.polution += (float)System.Math.Round(polutionCurveDetails.GetValueFromCurve((float)minute / 1440), 2);

        factorsManager.currentFactors.polution =
            HelperUtilities.LimitValueToTargetRange(Settings.minimumPolutionPoints, Settings.maximumPolutionPoints, factorsManager.currentFactors.polution);
    }

}
