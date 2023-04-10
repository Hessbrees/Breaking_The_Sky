using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zenject;

public class LightingControlManager : MonoBehaviour
{
    [SerializeField] private CurveDetailsSO curveDetails;
    [SerializeField] private GradientDetailsSO gradientDetails;

    [Inject(Id = "Factors")]
    private FactorsManager factorManager;

    [Inject(Id = "TimeManager")]
    private TimeChangeEvent timeChangeEvent;

    [Inject(Id = "GlobalLight")]
    private Light2D light2D;

    [Inject(Id = "GlobalLight")]
    private Volume globalVolume;

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

        TryChangeGlobalVolume();
    }
    private void TryChangeLightColor(int minute)
    {
        light2D.color = gradientDetails.GetColorFromGradient((float)minute / 1440f);
    }
    private void TryChangeLightIntensity(int minute)
    {
        light2D.intensity = Mathf.Abs(GetLightByDayTime(minute)-GetLightByPolution());
    }
    private void TryChangeGlobalVolume()
    {
        Vignette vignette;

        globalVolume.profile.TryGet(out vignette);

        vignette.intensity.Override(Mathf.Lerp(0.25f, 1f, factorManager.currentFactors.polution / Settings.maximumPolutionPoints));
    }
    private float GetLightByDayTime(int minute)
    {
        return curveDetails.GetValueFromCurve((float)minute / 1440f);
    }

    private float GetLightByPolution()
    {
        return Settings.maxLightDecraseIntensityByPolution * (factorManager.currentFactors.polution / Settings.maximumPolutionPoints);
    }


}
