using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

public class PolutionControl : MonoBehaviour
{
    [Inject(Id = "GlobalLight")]
    private Light2D globalLight;

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

    }

    private void TryChangePolution()
    {

    }
}
