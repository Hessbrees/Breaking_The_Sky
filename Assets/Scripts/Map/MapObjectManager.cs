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

    private Queue<GameObject> objectToAddQueue = new Queue<GameObject>();
    private Queue<GameObject> objectToRemoveQueue = new Queue<GameObject>();


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
        for (int x = tileMapBoards[0]; x < tileMapBoards[1]; x++)
            for (int y = tileMapBoards[0]; y < tileMapBoards[1]; y++)
            {
                availablePositions.Add(new Vector2(x, y));
            }
    }

    public void UpdateObjectInMap(Factors factors)
    {
        if (availablePositions.Count == 0) return;

        foreach (var blockNode in startObjectsList)
        {
            // get random position from list
            int randomIndex = Random.Range(0, availablePositions.Count);

            Vector2 position = availablePositions[randomIndex];

            // remove available position from list if object is instantiated succefully
            if (TryInstantiateNewObjectInMap(blockNode, factors, position))
                availablePositions.RemoveAt(randomIndex);

        }

        foreach (var obj in currentMapObjectList)
        {
            ObjectSpawnSettings objectSettings = obj.GetComponent<ObjectSpawnSettings>();

            BlockNodeGraphSO blockNodeGraph = blockNodeGraphList[objectSettings.blockNodeGraphID];

            BlockNodeSO currentBlockNode = GetCurrentNode(objectSettings, blockNodeGraph);

            for (int i = 0; i < currentBlockNode.childBlockNodeIDList.Count; i++)
                if (TryUpdateObjectInMap(obj, GetNextNode(currentBlockNode,i,blockNodeGraph), factors, i, blockNodeGraph))
                    break;

        }

        while(objectToAddQueue.Count > 0)
        {
            currentMapObjectList.Add(objectToAddQueue.Dequeue());
        }

        while(objectToRemoveQueue.Count > 0)
        {
            currentMapObjectList.Remove(objectToRemoveQueue.Dequeue());
        }
    }
    private bool TryInstantiateNewObjectInMap(BlockNodeSO blockNode, Factors factors, Vector2 position)
    {
        int randomSpawnChance = Random.Range(0, 1);

        if (blockNode.startBlockNode.GetSpawnChance(factors) > randomSpawnChance)
        {
            GameObject newGameObject = Instantiate(blockNode.startBlockNode.startBlockPrefab, spawnedObjectsParent);
            newGameObject.transform.localPosition = position;

            // object settings
            ObjectSpawnSettings objectSettings = newGameObject.GetComponent<ObjectSpawnSettings>();

            objectSettings.objectID = blockNode.id;

            for (int i = 0; i < blockNodeGraphList.Count; i++)
                if (blockNode.blockNodeGraph == blockNodeGraphList[i])
                {
                    objectSettings.blockNodeGraphID = i;
                    break;
                }

            objectSettings.pointsToChangePrefab = new int[blockNode.childBlockNodeIDList.Count];

            objectSettings.spawnPosition = position;

            currentMapObjectList.Add(newGameObject);

            return true;
        }

        return false;
    }
    private bool TryUpdateObjectInMap(GameObject targetObject, BlockNodeSO connectBlockNode, Factors factors, int currentIndex, BlockNodeGraphSO blockNodeGraph)
    {
        if (connectBlockNode.connectBlockNode.IsFactorsRequirementsAreMet(factors))
        {
            ObjectSpawnSettings objectSettings = targetObject.GetComponent<ObjectSpawnSettings>();

            if (objectSettings.pointsToChangePrefab[currentIndex] >= connectBlockNode.connectBlockNode.connectBlockPoints)
            {
                BlockNodeSO nextBlockNode = GetNextNode(connectBlockNode, blockNodeGraph);

                if (nextBlockNode.blockNodeType.isStepBlock)
                {
                    ChangeGameObject(targetObject, nextBlockNode.stepBlockNode.stepBlockPrefab, objectSettings.spawnPosition, nextBlockNode);
                }
                else if(nextBlockNode.blockNodeType.isEndBlock)
                {
                    RemoveGameObject(targetObject,objectSettings);
                }

                return true;
            }
            objectSettings.pointsToChangePrefab[currentIndex] += Random.Range(0, 4);

        }

        return false;
    }

    // give next connect node
    private BlockNodeSO GetCurrentNode(ObjectSpawnSettings objectSettings, BlockNodeGraphSO blockNodeGraph)
    {
        // find block in graph list and return
        foreach (BlockNodeSO item in blockNodeGraph.blockNodeList)
        {
            if (item.id == objectSettings.objectID)
            {
                return item;
            }
        }

        return null;
    }
    // give next node after connect node
    private BlockNodeSO GetNextNode(BlockNodeSO blockNode, int currentIndex, BlockNodeGraphSO blockNodeGraph)
    {
        // find block in graph list and return
        foreach (BlockNodeSO item in blockNodeGraph.blockNodeList)
        {
            if (blockNode.childBlockNodeIDList[currentIndex] == item.id)
            {
                return item;
            }
        }

        return null;

    }
    // find next block node or end node from child in connect node
    private BlockNodeSO GetNextNode(BlockNodeSO connectNode, BlockNodeGraphSO blockNodeGraph)
    {

        foreach (BlockNodeSO item in blockNodeGraph.blockNodeList)
            if (connectNode.childBlockNodeIDList[0] == item.id)
            {
                return item;
            }


        return null;
    }
    private void RemoveGameObject(GameObject gameObject,ObjectSpawnSettings objectSpawnSettings)
    {
        objectToRemoveQueue.Enqueue(gameObject);

        availablePositions.Add(objectSpawnSettings.spawnPosition);

        GameObject.Destroy(gameObject);
    }
    private void ChangeGameObject(GameObject gameObject, GameObject newPrefab, Vector2 position,BlockNodeSO blockNode)
    {
        objectToRemoveQueue.Enqueue(gameObject);

        GameObject.Destroy(gameObject);

        GameObject newGameObject = Instantiate(newPrefab, spawnedObjectsParent);

        newGameObject.transform.localPosition = position;

        // object settings
        ObjectSpawnSettings objectSettings = newGameObject.GetComponent<ObjectSpawnSettings>();

        objectSettings.objectID = blockNode.id;

        for (int i = 0; i < blockNodeGraphList.Count; i++)
            if (blockNode.blockNodeGraph == blockNodeGraphList[i])
            {
                objectSettings.blockNodeGraphID = i;
                break;
            }

        objectSettings.pointsToChangePrefab = new int[blockNode.childBlockNodeIDList.Count];

        objectSettings.spawnPosition = position;

        objectToAddQueue.Enqueue(newGameObject);
    }
}
