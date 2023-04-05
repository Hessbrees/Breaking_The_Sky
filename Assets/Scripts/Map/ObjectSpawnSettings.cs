using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawnSettings : MonoBehaviour
{
    [HideInInspector] public string objectID;
    [HideInInspector] public int blockNodeGraphID;
    [HideInInspector] public int[] pointsToChangePrefab;
    [HideInInspector] public Vector2 spawnPosition;
}
