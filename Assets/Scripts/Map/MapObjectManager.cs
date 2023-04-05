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
    public List<Vector2> availablePositions;
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
    public void FillAvailablePositionList(int[] tileMapBoards)
    {
        for(int x = tileMapBoards[0]; x < tileMapBoards[1] ; x++)
            for (int y = tileMapBoards[0]; y < tileMapBoards[1]; y++)
            {
                availablePositions.Add(new Vector2(x,y));
            }
    }

    public void UpdateObjectInMap(Factors factors)
    {
        if(availablePositions.Count == 0) return;

        foreach (var blockNode in startObjectsList)
        {
            // get random position from list
            int randomIndex = Random.Range(0, availablePositions.Count);

            Vector2 position = availablePositions[randomIndex];
             
            // remove available position from list if object is instantiated succefully
            if(TryInstantiateNewObjectInMap(blockNode, factors, position))
                availablePositions.RemoveAt(randomIndex);
            
        }
    }
    private bool TryInstantiateNewObjectInMap(BlockNodeSO blockNode, Factors factors, Vector2 position)
    {
        int randomSpawnChance = Random.Range(0, 1);

        if (blockNode.startBlockNode.GetSpawnChance(factors) > randomSpawnChance)
        {
            GameObject newGameObject = Instantiate(blockNode.startBlockNode.startBlockPrefab, spawnedObjectsParent);
            newGameObject.transform.localPosition = position;
            currentMapObjectList.Add(newGameObject);
            
            return true;
        }

        return false;
    }

}
