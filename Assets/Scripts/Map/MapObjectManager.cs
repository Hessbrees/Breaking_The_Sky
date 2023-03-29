using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;
using static UnityEditor.Progress;

[DisallowMultipleComponent]
public class MapObjectManager : MonoBehaviour
{
    [SerializeField] private List<BlockNodeGraphSO> blockNodeGraphList;
    [SerializeField] Transform spawnedObjectsParent;

    private List<BlockNodeSO> startObjectsList = new List<BlockNodeSO>();
    private List<GameObject> currentMapObjectList = new List<GameObject>();

    private void Awake()
    {
        GetStartObjects();
    }

    private void GetStartObjects()
    {
        Queue<BlockNodeSO> startObjectQueue = new Queue<BlockNodeSO>();

        foreach (BlockNodeGraphSO obj in blockNodeGraphList)
        {
            foreach (var item in obj.blockNodeList)
            {
                if (item.blockNodeType.isStartBlock)
                {                  
                    startObjectQueue.Enqueue(item);
                }
            }
        }

        while (startObjectQueue.Count > 0)
        {
            startObjectsList.Add(startObjectQueue.Dequeue());
        }


    }
    public void UpdateObjectInMap(float temperature, float pollution, float radiation, int[] tileMapBorder)
    {
        foreach (var blockNode in startObjectsList)
        {
            int xPosition = Random.Range(tileMapBorder[0], tileMapBorder[1] + 1);
            int yPosition = Random.Range(tileMapBorder[2], tileMapBorder[3] + 1);

            TryInstantiateNewObjectInMap(blockNode, temperature, pollution, radiation, new Vector2(xPosition, yPosition));
        }
    }
    private void TryInstantiateNewObjectInMap(BlockNodeSO blockNode, float temperature, float pollution, float radiation, Vector2 position)
    {
        int randomSpawnChance = Random.Range(0, 1);

        if (blockNode.startBlockNode.GetSpawnChance(temperature, pollution, radiation) > randomSpawnChance)
        {
            GameObject newGameObject = Instantiate(blockNode.startBlockNode.startBlockPrefab, spawnedObjectsParent);
            newGameObject.transform.position = position;
            currentMapObjectList.Add(newGameObject);
        }
    }

}
