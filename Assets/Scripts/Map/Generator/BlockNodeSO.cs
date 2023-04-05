using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using UnityEditor.TerrainTools;

[CreateAssetMenu(fileName = "BlockNode_", menuName = "Scriptable Objects/Map/Block Node")]
public class BlockNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentBlockNodeIDList = new List<string>();
    [HideInInspector] public List<string> childBlockNodeIDList = new List<string>();
    [HideInInspector] public BlockNodeGraphSO blockNodeGraph;
    public BlockNodeTypeSO blockNodeType;
    [HideInInspector] public BlockNodeTypeListSO blockNodeTypeList;
    public ConnectBlockNode connectBlockNode;
    [HideInInspector] public StepBlockNode stepBlockNode;
    [HideInInspector] public StartBlockNode startBlockNode;


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
    private const float startNodeWidth = 250f;
    private const float startNodeHeight = 450f;
    private const float connectNodeWidth = 250f;
    private const float connectNodeHeight = 400f;
    private const float stepNodeWidth = 250f;
    private const float stepNodeHeight = 150f;
    private const float BlockNodeSelectionHeight = 15f;

    /// <summary>
    /// Initialise node
    /// </summary>
    public void Initialise(Rect rect, BlockNodeGraphSO nodeGraph, BlockNodeTypeSO blockNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "BlockNode";
        this.blockNodeGraph = nodeGraph;
        this.blockNodeType = blockNodeType;
        this.selectableRect = GetBlockSelectablePlace();

        // Load block node type list
        blockNodeTypeList = GameResources.Instance.blockNodeTypeList;
    }
    public Rect GetBlockSelectablePlace()
    {
        return new Rect(rect.position.x + GetBlockRect(rect).width/2 - (GetBlockRect(rect).width - 40) /2, rect.position.y + 10, GetBlockRect(rect).width -40, BlockNodeSelectionHeight);
    }
    public Rect GetBlockRect(Rect rect)
    {
        if (blockNodeType.isStartBlock)
        {
            return new Rect(rect.position.x, rect.position.y, startNodeWidth, startNodeHeight);
        }
        else if (blockNodeType.isConnectBlock)
        {
            return new Rect(rect.position.x, rect.position.y, connectNodeWidth, connectNodeHeight);
        }
        else if(blockNodeType.isStepBlock)
        {
            return new Rect(rect.position.x, rect.position.y, stepNodeWidth, stepNodeHeight);
        }
        else return rect;
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

        // if the block node has a parent or is of type start then display a label else display a popup
        if (parentBlockNodeIDList.Count > 0 || blockNodeType.isStartBlock)
        {

            EditorGUILayout.LabelField(blockNodeType.blockNodeTypeName, titleStyle);
        }
        else
        {
            // Display a popup using the BlockNodeType name values that can be selected from
            int selected = blockNodeTypeList.list.FindIndex(x => x == blockNodeType);

            int selection = EditorGUILayout.Popup("", selected, GetBlockNodeTypesToDisplay());

            blockNodeType = blockNodeTypeList.list[selection];
        }

        if (blockNodeType.isStartBlock)
        {
            if (startBlockNode == null)
            {
                CreateStartBlockNode(this);
            }

            startBlockNode.DrawStartBlock();
        }

        if (blockNodeType.isConnectBlock)
        {
            if (connectBlockNode == null)
            {
                CreateConnectBlockNode(this);
            }
            connectBlockNode.DrawConnectBlock();
        }

        if (blockNodeType.isStepBlock)
        {
            if (stepBlockNode == null)
            {
                CreateStepBlockNode(this);
            }
            stepBlockNode.DrawStepBlock();
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        GUILayout.EndArea();

        GUILayout.BeginArea(selectableRect, selectableStyle);
        GUILayout.EndArea();
    }
    public void CreateStartBlockNode(BlockNodeSO blockNode)
    {
        // create start block node
        startBlockNode = new StartBlockNode();

        // initialize new start block node component
        this.name = "StartBlockNode";

        // save changes in project
        AssetDatabase.SaveAssets();
    }

    public void CreateConnectBlockNode(BlockNodeSO blockNode)
    {
        // create start block node
        connectBlockNode = new ConnectBlockNode();

        // initialize new connect block node component
        this.name = "ConnectBlockNode";

        // save changes in project
        AssetDatabase.SaveAssets();
    }
    public void CreateStepBlockNode(BlockNodeSO blockNode)
    {
        // create start block node
        stepBlockNode = new StepBlockNode();

        // initialize new connect block node component
        this.name = "StepBlockNode";

        // save changes in project
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// Populate a string array with the block node types to display that can be selected
    /// </summary>
    public string[] GetBlockNodeTypesToDisplay()
    {
        string[] blockArray = new string[blockNodeTypeList.list.Count];

        for (int i = 0; i < blockNodeTypeList.list.Count; i++)
        {
            if (blockNodeTypeList.list[i].displayInBlockGraphEditor)
            {
                blockArray[i] = blockNodeTypeList.list[i].blockNodeTypeName;
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
        blockNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
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
    public bool AddChildBlockNodeIDToBlockNode(string childID)
    {
        // Check child node can be added validly to parent
        if (IsChildBlockValid(childID))
        {
            childBlockNodeIDList.Add(childID);
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
        if (blockNodeGraph.GetBlockNode(childID).blockNodeType.isNone)
            return false;

        // If the node already has a child with this child ID return false
        if (childBlockNodeIDList.Contains(childID))
            return false;

        // If this childID is already in the parentID list return false 
        if (parentBlockNodeIDList.Contains(childID))
            return false;

        // If the child node already has a parent return false
        if (blockNodeGraph.GetBlockNode(childID).parentBlockNodeIDList.Count > 0 && !blockNodeGraph.GetBlockNode(childID).blockNodeType.isConnectBlock && !blockNodeGraph.GetBlockNode(childID).blockNodeType.isEndBlock)
            return false;

        // If child is a connect block and this node is a connect block return false
        if (blockNodeGraph.GetBlockNode(childID).blockNodeType.isConnectBlock && blockNodeType.isConnectBlock)
            return false;

        // If child is not a connect block and this node is not a connect block return false
        if (!blockNodeGraph.GetBlockNode(childID).blockNodeType.isConnectBlock && !blockNodeType.isConnectBlock)
            return false;

        // If the child block is an start block return false - the start block must always be the top level parent node
        if (blockNodeGraph.GetBlockNode(childID).blockNodeType.isStartBlock)
            return false;

        // If adding a block to connect block check that this connect block doesn't already have a block added
        if (!blockNodeGraph.GetBlockNode(childID).blockNodeType.isConnectBlock && childBlockNodeIDList.Count > 0)
            return false;

        // If this is a end block return false
        if (blockNodeType.isEndBlock)
            return false;

        return true;

    }
    /// <summary>
    /// Add parentID to the node (returns true if the node has been added, false otherwise)
    /// </summary>
    public bool AddParentBlockNodeIDToBlockNode(string parentID)
    {
        parentBlockNodeIDList.Add(parentID);
        return true;
    }


    /// <summary>
    /// Remove childID from the node (returns true if the node has been removed, false otherwise)
    /// </summary>
    public bool RemoveChildBlockNodeIDFromBlockNode(string childID)
    {
        // if the node contains the child ID then remove it
        if (childBlockNodeIDList.Contains(childID))
        {
            childBlockNodeIDList.Remove(childID);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Remove parentID from the node (returns true if the node has been remove, false otherwise)
    /// </summary>
    public bool RemoveParentBlockNodeIDFromBlockNode(string parentID)
    {
        // if the node contains the parent ID then remove it
        if (parentBlockNodeIDList.Contains(parentID))
        {
            parentBlockNodeIDList.Remove(parentID);
            return true;
        }
        return false;
    }



#endif
    #endregion Editor Code

}
