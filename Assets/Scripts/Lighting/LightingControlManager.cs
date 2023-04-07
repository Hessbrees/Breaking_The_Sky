using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

public class LightingControlManager : MonoBehaviour
{
    [SerializeField] private LightNodeGraphSO lightingGraph;

    LightNode currentLightingNode;
    LightNode nextLightingNode;

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
        if (CheckActualLightingNodes(ConvertTimeToMinutes(timeChangeArg.gameHour, timeChangeArg.gameMinute)))
        {
            light2D.intensity = currentLightingNode.lightingBrightness.lightIntensity;
            light2D.color = currentLightingNode.lightingBrightness.color;
        }


    }
    private int ConvertTimeToMinutes(int hour, int minute)
    {
        return hour * 60 + minute;
    }
    private bool CheckActualLightingNodes(float timeMinutes)
    {
        if (currentLightingNode == null || nextLightingNode == null)
        {
            FindLightNodes(timeMinutes);
            return true;
        }

        if (nextLightingNode.GetTimeInMinutes() > timeMinutes && timeMinutes !=0) return false;

        FindLightNodes(timeMinutes);

        return true;
    }
    private void FindLightNodes(float timeMinutes)
    {
        float currentMinutes = 1440;

        foreach (LightNodeSO lightNodeSO in lightingGraph.lightNodeList)
        {
            if (timeMinutes < lightNodeSO.lightNode.GetTimeInMinutes() && currentMinutes > lightNodeSO.lightNode.GetTimeInMinutes())
            {
                nextLightingNode = lightNodeSO.lightNode;
                currentLightingNode = lightNodeSO.lightNodeGraph.GetLightNode(lightNodeSO.parentLightNodeIDList[0]).lightNode;
                currentMinutes = lightNodeSO.lightNode.GetTimeInMinutes();

            }
        }
    }
}
