using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "LightNodeGraph_", menuName = "Scriptable Objects/Lighting/Light Node Graph Editor")]
public class LightNodeGraphSO : ScriptableObject
{
    [HideInInspector] public LightNodeTypeListSO lightNodeListSO;
    [HideInInspector] public List<LightNodeSO> lightNodeList = new List<LightNodeSO>();
    [HideInInspector] public Dictionary<string, LightNodeSO> lightNodeDictionary = new Dictionary<string, LightNodeSO>();

    private void Awake()
    {
        LoadLightNodeDictionary();

    }

    /// <summary>
    /// Load the light node dictionary from the light node list.
    /// </summary>
    private void LoadLightNodeDictionary()
    {
        lightNodeDictionary.Clear();

        // Populate dictionary
        foreach (LightNodeSO node in lightNodeList)
        {
            lightNodeDictionary[node.id] = node;
        }
    }
    /// <summary>
    /// Get light node by light nodeID
    /// </summary>
    public LightNodeSO GetLightNode(string lightNodeID)
    {
        if (lightNodeDictionary.TryGetValue(lightNodeID, out LightNodeSO lightNode))
        {
            return lightNode;
        }
        return null;
    }
    #region Editor Code

#if UNITY_EDITOR

    [HideInInspector] public LightNodeSO lightNodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 linePosition;

    // Repopulate node dictionary every time a change is made in the editor
    public void OnValidate()
    {
        LoadLightNodeDictionary();
    }

    public void SetNodeToDrawConnectionLineFrom(LightNodeSO node, Vector2 position)
    {
        lightNodeToDrawLineFrom = node;
        linePosition = position;
    }

#endif

    #endregion Editor Code
}
