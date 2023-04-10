using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

public class LightingControlManager : MonoBehaviour
{
    [SerializeField] private CurveDetailsSO curveDetails;
    [SerializeField] private GradientDetailsSO gradientDetails;

    [Inject(Id = "TimeManager")]
    private TimeChangeEvent timeChangeEvent;

    [Inject(Id = "GlobalLight")]
    private Light2D light2D;

    private void OnEnable()
    {
        timeChangeEvent.OnTimeChange += TryChangeLight_OnTimeChange;
    }
    private void OnDisable()
    {

        timeChangeEvent.OnTimeChange -= TryChangeLight_OnTimeChange;
    }

    private void TryChangeLight_OnTimeChange(TimeChangeEvent timeChangeEvent, TimeChangeArg timeChangeArg)
    {
        int timeMinute = HelperUtilities.GetTimeInMinutes(timeChangeArg.gameHour, timeChangeArg.gameMinute);

        TryChangeLightIntensity(timeMinute);

        TryChangeLightColor(timeMinute);
    }
    private void TryChangeLightColor(int minute)
    {
        light2D.color = gradientDetails.GetColorFromGradient((float)minute / 1440f);
    }
    private void TryChangeLightIntensity(int minute)
    {
        light2D.intensity = GetLightByDayTime(minute);
    }

    private float GetLightByDayTime(int minute)
    {
        return curveDetails.GetValueFromCurve((float)minute / 1440f);
    }
}
