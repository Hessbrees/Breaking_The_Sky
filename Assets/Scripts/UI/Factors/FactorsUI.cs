using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class FactorsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI temperature;
    [SerializeField] private TextMeshProUGUI polution;
    [SerializeField] private TextMeshProUGUI radiation;

    [Inject(Id ="Factors")]
    private FactorsManager factorsManager;

    [Inject(Id ="TimeManager")]
    private TimeChangeEvent timeChangeEvent;
    private void OnEnable()
    {
        timeChangeEvent.OnTimeChange += ChangeTextFactorsValue_OnTimeChange;
    }
    private void OnDisable()
    {
        timeChangeEvent.OnTimeChange -= ChangeTextFactorsValue_OnTimeChange;
    }
    private void ChangeTextFactorsValue_OnTimeChange(TimeChangeEvent timeChangeEvent, TimeChangeArg timeChangeArg)
    {
        temperature.text = (factorsManager.currentFactors.temperature -40).ToString("f0");
        polution.text = factorsManager.currentFactors.polution.ToString("f0");
        radiation.text = factorsManager.currentFactors.radiation.ToString("f0");
    }
}
