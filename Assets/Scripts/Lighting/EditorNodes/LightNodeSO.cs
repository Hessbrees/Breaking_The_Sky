using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "LightNode_", menuName = "Scriptable Objects/Lighting/Light Node")]
public class LightNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentLightNodeIDList = new List<string>();
    [HideInInspector] public List<string> childLightNodeIDList = new List<string>();
    [HideInInspector] public LightNodeGraphSO lightNodeGraph;
    [HideInInspector] public LightNodeTypeSO lightNodeType;
    [HideInInspector] public LightNodeTypeListSO lightNodeTypeList;
    [HideInInspector] public LightNode lightNode;

    #region Editor Code
#if UNITY_EDITOR

    [HideInInspector] public Rect rect;
    [HideInInspector] public Rect selectableRect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;

    // Block Nodes Position
    [HideInInspector] public bool isNodesPositionBlocked = false;
    [HideInInspector] public bool isNodesSelectionBlocked = false;


    // Start Node layout values
    private const float nodeWidth = 250f;
    private const float nodeHeight = 270f;

    private const float lightNodeSelectionHeight = 15f;

    /// <summary>
    /// Initialise node
    /// </summary>
    public void Initialise(Rect rect, LightNodeGraphSO nodeGraph, LightNodeTypeSO lightNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "LightNode";
        this.lightNodeGraph = nodeGraph;
        this.lightNodeType = lightNodeType;
        this.selectableRect = GetBlockSelectablePlace();

        // Load light node type list
        lightNodeTypeList = GameResources.Instance.lightNodeTypeList;


        // Initialize light node class
        if (lightNodeType.isEndBlock)
        {
            CreateEndLightNode();

        }
        else
        {
            CreateLightNode();
        }


    }
    public Rect GetBlockSelectablePlace()
    {
        return new Rect(rect.position.x + GetBlockRect(rect).width / 2 - (GetBlockRect(rect).width - 40) / 2, rect.position.y + 10, GetBlockRect(rect).width - 40, lightNodeSelectionHeight);
    }
    public Rect GetBlockRect(Rect rect)
    {
        return new Rect(rect.position.x, rect.position.y, nodeWidth, nodeHeight);

    }

    /// <summary>
    /// Draw node with the nodestyle
    /// </summary>
    public void Draw(GUIStyle nodeStyle, GUIStyle titleStyle, GUIStyle selectableStyle)
    {

        // Draw Node Box Using Begin Area
        GUILayout.BeginArea(GetBlockRect(rect), nodeStyle);

        // check selectable rect change
        selectableRect = GetBlockSelectablePlace();

        // Start Region To Detect Popup Selection Changes
        EditorGUI.BeginChangeCheck();

        // if the light node has a parent or is of type start then display a label else display a popup
        if (parentLightNodeIDList.Count > 0 || lightNodeType.isStartBlock || lightNodeType.isEndBlock)
        {
            EditorGUILayout.LabelField(lightNodeType.lightNodeTypeName, titleStyle);
        }
        else
        {
            // Display a popup using the LightNodeType name values that can be selected from
            int selected = lightNodeTypeList.list.FindIndex(x => x == lightNodeType);

            int selection = EditorGUILayout.Popup("", selected, GetLightNodeTypesToDisplay());

            lightNodeType = lightNodeTypeList.list[selection];
        }


        lightNode.DrawLightNode(lightNodeType);

        if (childLightNodeIDList.Count > 0 && parentLightNodeIDList.Count > 0)
            lightNode.DrawVerificationNote(lightNodeGraph.GetLightNode(childLightNodeIDList[0]), lightNodeGraph.GetLightNode(parentLightNodeIDList[0]));

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        GUILayout.EndArea();

        GUILayout.BeginArea(selectableRect, selectableStyle);
        GUILayout.EndArea();
    }

    public void CreateLightNode()
    {
        // create start block node
        lightNode = new LightNode();

        // save changes in project
        AssetDatabase.SaveAssets();
    }
    public void CreateEndLightNode()
    {
        LightNodeSO startNode = null;

        foreach (LightNodeSO node in lightNodeGraph.lightNodeList)
            if (node.lightNodeType.isStartBlock)
            {
                startNode = node;
                break;
            }

        // create start block node
        lightNode = new LightNode(startNode);

        // save changes in project
        AssetDatabase.SaveAssets();
    }
    /// <summary>
    /// Populate a string array with the light node types to display that can be selected
    /// </summary>
    public string[] GetLightNodeTypesToDisplay()
    {
        string[] blockArray = new string[lightNodeTypeList.list.Count];

        for (int i = 0; i < lightNodeTypeList.list.Count; i++)
        {
            if (lightNodeTypeList.list[i].displayInBlockGraphEditor)
            {
                blockArray[i] = lightNodeTypeList.list[i].lightNodeTypeName;
            }
        }

        return blockArray;
    }

    /// <summary>
    /// Process events for the node
    /// </summary>
    public void ProcessEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            // Process Mouse Down Events
            case EventType.MouseDown:
                if (!isNodesSelectionBlocked) ProcessMouseDownEvent(currentEvent);
                break;

            // Process Mouse Up Events
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;

            // Process Mouse Drag Events
            case EventType.MouseDrag:
                if (!isNodesPositionBlocked) ProcessMouseDragEvent(currentEvent);
                break;

            default:
                break;
        }
    }

    /// Process mouse down events
    /// </summary>
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // left click down
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDownEvent();
        }
        // right click down
        else if (currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);
        }
    }
    /// <summary>
    /// Process left click down event
    /// </summary>
    private void ProcessLeftClickDownEvent()
    {
        Selection.activeObject = this;

        // Toggle node selection
        if (isSelected == true)
        {
            isSelected = false;
        }
        else
        {
            isSelected = true;
        }
    }
    /// <summary>
    /// Process right click down
    ///
    private void ProcessRightClickDownEvent(Event currentEvent)
    {
        lightNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
    }
    /// <summary>
    /// Process mouse up event
    /// </summary>
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        // If left click up
        if (currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent();
        }
    }

    /// <summary>
    /// Process left click up event
    /// </summary>
    private void ProcessLeftClickUpEvent()
    {
        if (isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
    }

    /// <summary>
    /// Process mouse drag event
    /// </summary>
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        // process left click drag event
        if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent);
        }
    }

    /// <summary>
    /// Process left mouse drag event
    /// </summary>
    private void ProcessLeftMouseDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;

        DragNode(currentEvent.delta);
        GUI.changed = true;
    }
    /// <summary>
    /// Drag node
    /// </summary>
    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        selectableRect.position += delta;
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// Add childID to the node (returns true if the node has been added, false otherwise)
    /// </summary>
    public bool AddChildLightNodeIDToLightNode(string childID)
    {
        // Check child node can be added validly to parent
        if (IsChildBlockValid(childID))
        {
            childLightNodeIDList.Add(childID);
            return true;
        }

        return false;
    }
    /// <summary>
    /// Check the child node can be validly added to the parent node - return true if it can otherwise return false
    /// </summary>
    public bool IsChildBlockValid(string childID)
    {

        // If this node ID and the child ID are the same return false
        if (id == childID)
            return false;

        // If the child node has a type of none then return false
        if (lightNodeGraph.GetLightNode(childID).lightNodeType.isNone)
            return false;

        // If the node already has a child with this child ID return false
        if (childLightNodeIDList.Contains(childID))
            return false;

        // If the child node already has a parent return false
        if (lightNodeGraph.GetLightNode(childID).parentLightNodeIDList.Count > 0)
            return false;

        // If the node has alredy child 
        if (childLightNodeIDList.Count > 0)
            return false;

        // If this childID is already in the parentID list return false 
        if (parentLightNodeIDList.Contains(childID))
            return false;

        // If the child light node is an start light node return false - the start light must always be the top level parent node
        if (lightNodeGraph.GetLightNode(childID).lightNodeType.isStartBlock && !lightNodeType.isEndBlock)
            return false;

        // If the parent light node is an end light node and child is not start node return false
        if (lightNodeType.isEndBlock && !lightNodeGraph.GetLightNode(childID).lightNodeType.isStartBlock)
            return false;

        return true;

    }
    /// <summary>
    /// Add parentID to the node (returns true if the node has been added, false otherwise)
    /// </summary>
    public bool AddParentLightNodeIDToLightNode(string parentID)
    {
        parentLightNodeIDList.Add(parentID);
        return true;
    }


    /// <summary>
    /// Remove childID from the node (returns true if the node has been removed, false otherwise)
    /// </summary>
    public bool RemoveChildLightNodeIDFromLightNode(string childID)
    {
        // if the node contains the child ID then remove it
        if (childLightNodeIDList.Contains(childID))
        {
            childLightNodeIDList.Remove(childID);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Remove parentID from the node (returns true if the node has been remove, false otherwise)
    /// </summary>
    public bool RemoveParentLightNodeIDFromLightNode(string parentID)
    {
        // if the node contains the parent ID then remove it
        if (parentLightNodeIDList.Contains(parentID))
        {
            parentLightNodeIDList.Remove(parentID);
            return true;
        }
        return false;
    }



#endif
    #endregion Editor Code

}
