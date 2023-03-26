using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockNodeGraph_", menuName = "Scriptable Objects/Map/Block Node Graph Editor")]
public class BlockNodeGraphSO : ScriptableObject
{
    [HideInInspector] public BlockNodeTypeListSO blockNodeListSO;
    [HideInInspector] public List<BlockNodeSO> blockNodeList = new List<BlockNodeSO>();
    [HideInInspector] public Dictionary<string, BlockNodeSO> blockNodeDictionary = new Dictionary<string, BlockNodeSO>();

    private void Awake()
    {
        LoadBlockNodeDictionary();

    }

    /// <summary>
    /// Load the block node dictionary from the block node list.
    /// </summary>
    private void LoadBlockNodeDictionary()
    {
        blockNodeDictionary.Clear();

        // Populate dictionary
        foreach (BlockNodeSO node in blockNodeList)
        {
            blockNodeDictionary[node.id] = node;
        }
    }
    /// <summary>
    /// Get block node by block nodeID
    /// </summary>
    public BlockNodeSO GetBlockNode(string blockNodeID)
    {
        if (blockNodeDictionary.TryGetValue(blockNodeID, out BlockNodeSO blockNode))
        {
            return blockNode;
        }
        return null;
    }
    #region Editor Code

#if UNITY_EDITOR

    [HideInInspector] public BlockNodeSO blockNodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 linePosition;

    // Repopulate node dictionary every time a change is made in the editor
    public void OnValidate()
    {
        LoadBlockNodeDictionary();
    }

    public void SetNodeToDrawConnectionLineFrom(BlockNodeSO node, Vector2 position)
    {
        blockNodeToDrawLineFrom = node;
        linePosition = position;
    }

#endif

    #endregion Editor Code

}
