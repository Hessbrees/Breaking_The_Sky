using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class BlockNodeGraphEditor : EditorWindow
{
    private GUIStyle blockNodeStyle;
    private GUIStyle blockNodeSelectedStyle;
    private GUIStyle blockNodeSelectablePlaceStyle;
    private GUIStyle titleStyle;
    private static BlockNodeGraphSO currentBlockNodeGraph;

    private Vector2 graphOffset;
    private Vector2 graphDrag;

    private BlockNodeSO currentBlockNode = null;
    private BlockNodeTypeListSO blockNodeTypeList;

    // Node layout values
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    // Connecting line values
    private const float connectingLineWidth = 3f;
    private const float connectingLineArrowSize = 6f;

    // Grid Spacing
    private const float gridLarge = 100f;
    private const float gridSmall = 25f;

    // Block Nodes Position
    private bool isNodesPositionBlocked = false;

    // Block Nodes Selection
    private bool isNodesSelectionBlocked = false;

    // Block Window Moving
    private bool isWindowMovingBlocked = false;

    // Move Screen By Arrows Strenght
    private float graphMoveStrenght = 40f;

    [MenuItem("Map Generator Graph Editor", menuItem = "Window/Map Editor/Map Generator Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<BlockNodeGraphEditor>("Map Generator Graph Editor");
    }

    [OnOpenAsset(0)]  // Need the namespace UnityEditor.Callbacks
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        BlockNodeGraphSO blockNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as BlockNodeGraphSO;

        if (blockNodeGraph != null)
        {
            OpenWindow();

            currentBlockNodeGraph = blockNodeGraph;

            return true;
        }
        return false;
    }

    private void OnEnable()
    {
        // Subscribe to the inspector selection changed event
        Selection.selectionChanged += InspectorSelectionChanged;

        // Define node layout style
        blockNodeStyle = new GUIStyle();
        blockNodeStyle.normal.textColor = Color.white;
        blockNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        blockNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        blockNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        // Define selected node style
        blockNodeSelectedStyle = new GUIStyle();
        blockNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        blockNodeSelectedStyle.normal.textColor = Color.white;
        blockNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        blockNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        Texture2D selectablePlaceTexture = Texture2D.grayTexture;
             
        blockNodeSelectablePlaceStyle = new GUIStyle();
        blockNodeSelectablePlaceStyle.normal.background = selectablePlaceTexture;

        titleStyle = new GUIStyle();
        titleStyle.normal.textColor = Color.green;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.fontSize = 18;

        // Load Block node types
        blockNodeTypeList = GameResources.Instance.blockNodeTypeList;
    }
    private void OnGUI()
    {

        // If a scriptable object of type BlockNodeGraphSO has been selected then process
        if (currentBlockNodeGraph != null)
        {
            VerticalControlers();

            // Draw Grid
            DrawBackgroundGrid(gridSmall, 0.2f, Color.gray);
            DrawBackgroundGrid(gridLarge, 0.3f, Color.gray);

            // Draw line if being dragged
            DrawDraggedLine();

            // Process Events
            ProcessEvents(Event.current);

            // Draw Connections Between block Nodes
            DrawblockConnections();

            // Draw block Nodes
            DrawBlockNodes();

        }

        if (GUI.changed)
        {
            Repaint();
        }

    }
    private void VerticalControlers()
    {
        GUILayout.BeginVertical();

        isNodesPositionBlocked = EditorGUILayout.Toggle("Block Nodes Position ", BlockNodesPosition());
        isNodesSelectionBlocked = EditorGUILayout.Toggle("Block Nodes Selection", BlockNodesSelection());
        isWindowMovingBlocked = EditorGUILayout.Toggle("Block Window Moving", BlockWindowMoving());

        GUILayout.EndVertical();
    }

    /// <summary>
    /// Draw a background grid for the block node graph editor
    /// </summary>
    private void DrawBackgroundGrid(float gridSize, float gridOpacity, Color gridColor)
    {
        int verticalLineCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);
        int horizontalLineCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);

        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        graphOffset += graphDrag * 0.5f;

        Vector3 gridOffset = new Vector3(graphOffset.x % gridSize, graphOffset.y % gridSize, 0);

        for (int i = 0; i < verticalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0) + gridOffset, new Vector3(gridSize * i, position.height + gridSize, 0f) + gridOffset);
        }

        for (int j = 0; j < horizontalLineCount; j++)
        {
            Handles.DrawLine(new Vector3(-gridSize, gridSize * j, 0) + gridOffset, new Vector3(position.width + gridSize, gridSize * j, 0f) + gridOffset);
        }

        Handles.color = Color.white;

    }
    private void DrawDraggedLine()
    {
        if (currentBlockNodeGraph.linePosition != Vector2.zero)
        {
            //Draw line from node to line position
            Handles.DrawBezier(currentBlockNodeGraph.blockNodeToDrawLineFrom.rect.center, currentBlockNodeGraph.linePosition, currentBlockNodeGraph.blockNodeToDrawLineFrom.rect.center, currentBlockNodeGraph.linePosition, Color.white, null, connectingLineWidth);
        }
    }

    private void ProcessEvents(Event currentEvent)
    {
        // Reset graph drag
        graphDrag = Vector2.zero;

        // Get block that mouse is over if it's null or not currently being dragged
        if (currentBlockNode == null || currentBlockNode.isLeftClickDragging == false)
        {
            currentBlockNode = IsMouseOverBlockNode(currentEvent);
        }

        // if mouse isn't over a block node
        if (currentBlockNode == null || currentBlockNodeGraph.blockNodeToDrawLineFrom != null)
        {
            ProcessBlockNodeGraphEvents(currentEvent);
        }
        // else process block node events
        else
        {
            // process block node events
            currentBlockNode.ProcessEvents(currentEvent);
        }

        ProcessKeyboardArrowsEvent(currentEvent);

    }
    /// <summary>
    ///  Check to see to mouse is over a block node - if so then return the block node else return null
    /// </summary>
    private BlockNodeSO IsMouseOverBlockNode(Event currentEvent)
    {
        for (int i = currentBlockNodeGraph.blockNodeList.Count - 1; i >= 0; i--)
        {
            if (currentBlockNodeGraph.blockNodeList[i].selectableRect.Contains(currentEvent.mousePosition))
            {
                return currentBlockNodeGraph.blockNodeList[i];
            }
        }

        return null;
    }
    private void ProcessBlockNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            // Process Mouse Down Events
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            // Process Mouse Up Events
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;

            // Process Mouse Drag Event
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
            default:
                break;
        }
    }
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
        else if (currentEvent.button == 0)
        {
            ClearLineDrag();
            ClearAllSelectedBlockNodes();
        }
    }
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Create Block Node"), false, CreateBlockNode, mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Select All Block Nodes"), false, SelectAllBlockNodes);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Selected Block Node Links"), false, DeleteSelectedBlockNodeLinks);
        menu.AddItem(new GUIContent("Delete Selected Block Nodes"), false, DeleteSelectedBlockNodes);
        menu.ShowAsContext();

    }

    private bool BlockWindowMoving()
    {
        isWindowMovingBlocked = !isWindowMovingBlocked;

        return isWindowMovingBlocked;
    }

    private bool BlockNodesSelection()
    {
        isNodesSelectionBlocked = !isNodesSelectionBlocked;

        foreach (BlockNodeSO blockNode in currentBlockNodeGraph.blockNodeList)
        {
            blockNode.isNodesSelectionBlocked = isNodesSelectionBlocked;
        }

        return isNodesSelectionBlocked;
    }
    private bool BlockNodesPosition()
    {
        isNodesPositionBlocked = !isNodesPositionBlocked;

        foreach (BlockNodeSO blockNode in currentBlockNodeGraph.blockNodeList)
        {
            blockNode.isNodesPositionBlocked = isNodesPositionBlocked;
        }

        return isNodesPositionBlocked;
    }

    /// <summary>
    /// Create a block node at the mouse position
    /// </summary>
    /// <param name="mousePositionObject"></param>
    private void CreateBlockNode(object mousePositionObject)
    {
        // If current node graph empty then add entrance block node first
        if (currentBlockNodeGraph.blockNodeList.Count == 0)
        {
            CreateBlockNode(new Vector2(200f, 200f), blockNodeTypeList.list.Find(x => x.isStartBlock));
        }

        CreateBlockNode(mousePositionObject, blockNodeTypeList.list.Find(x => x.isNone));
    }

    private void CreateBlockNode(object mousePositionObject, BlockNodeTypeSO blockNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        // create block node scriptable object asset
        BlockNodeSO blockNode = ScriptableObject.CreateInstance<BlockNodeSO>();

        // add block node to current block node graph block node list
        currentBlockNodeGraph.blockNodeList.Add(blockNode);

        // set block node values

        blockNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentBlockNodeGraph, blockNodeType);

        // add block node to block node graph scriptable object asset database
        AssetDatabase.AddObjectToAsset(blockNode, currentBlockNodeGraph);

        AssetDatabase.SaveAssets();

        // Refresh graph node dictionary
        currentBlockNodeGraph.OnValidate();

    }

    /// <summary>
    /// Delete selected block nodes
    /// </summary>
    private void DeleteSelectedBlockNodes()
    {
        Queue<BlockNodeSO> blockNodeDeletionQueue = new Queue<BlockNodeSO>();

        // Loop through all nodes
        foreach (BlockNodeSO blockNode in currentBlockNodeGraph.blockNodeList)
        {
            if (blockNode.isSelected)
            {
                blockNodeDeletionQueue.Enqueue(blockNode);

                // iterate through child block nodes ids
                foreach (string childBlockNodeID in blockNode.childBlockNodeIDList)
                {
                    // Retrieve child block node
                    BlockNodeSO childBlockNode = currentBlockNodeGraph.GetBlockNode(childBlockNodeID);

                    if (childBlockNode != null)
                    {
                        // Remove parentID from child block node
                        childBlockNode.RemoveParentBlockNodeIDFromBlockNode(blockNode.id);
                    }
                }

                // Iterate through parent block node ids
                foreach (string parentblockNodeID in blockNode.parentBlockNodeIDList)
                {
                    // Retrieve parent node
                    BlockNodeSO parentblockNode = currentBlockNodeGraph.GetBlockNode(parentblockNodeID);

                    if (parentblockNode != null)
                    {
                        // Remove childID from parent node
                        parentblockNode.RemoveChildBlockNodeIDFromBlockNode(blockNode.id);
                    }
                }
            }
        }

        // Delete queued block nodes
        while (blockNodeDeletionQueue.Count > 0)
        {
            // Get block node from queue
            BlockNodeSO blockNodeToDelete = blockNodeDeletionQueue.Dequeue();

            // Remove node from dictionary
            currentBlockNodeGraph.blockNodeDictionary.Remove(blockNodeToDelete.id);

            // Remove node from list
            currentBlockNodeGraph.blockNodeList.Remove(blockNodeToDelete);

            // Remove node from Asset database
            DestroyImmediate(blockNodeToDelete, true);

            // Save asset database
            AssetDatabase.SaveAssets();

        }
    }

    /// <summary>
    /// Delete the links between the selected block nodes
    /// </summary>
    private void DeleteSelectedBlockNodeLinks()
    {
        // Iterate through all block nodes
        foreach (BlockNodeSO blockNode in currentBlockNodeGraph.blockNodeList)
        {
            if (blockNode.isSelected && blockNode.childBlockNodeIDList.Count > 0)
            {
                for (int i = blockNode.childBlockNodeIDList.Count - 1; i >= 0; i--)
                {
                    // Get child block node
                    BlockNodeSO childblockNode = currentBlockNodeGraph.GetBlockNode(blockNode.childBlockNodeIDList[i]);

                    // If the child block node is selected
                    if (childblockNode != null && childblockNode.isSelected)
                    {
                        // Remove childID from parent block node
                        blockNode.RemoveChildBlockNodeIDFromBlockNode(childblockNode.id);

                        // Remove parentID from child block node
                        childblockNode.RemoveParentBlockNodeIDFromBlockNode(blockNode.id);
                    }
                }
            }
        }

        // Clear all selected block nodes
        ClearAllSelectedBlockNodes();
    }

    /// <summary>
    /// Clear selection from all block nodes
    /// </summary>
    private void ClearAllSelectedBlockNodes()
    {
        foreach (BlockNodeSO blockNode in currentBlockNodeGraph.blockNodeList)
        {
            if (blockNode.isSelected)
            {
                blockNode.isSelected = false;

                GUI.changed = true;
            }
        }
    }

    /// <summary>
    /// Select all block nodes
    /// </summary>
    private void SelectAllBlockNodes()
    {
        foreach (BlockNodeSO blockNode in currentBlockNodeGraph.blockNodeList)
        {
            blockNode.isSelected = true;
        }
        GUI.changed = true;
    }

    /// <summary>
    /// Process mouse up events
    /// </summary>
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        // if releasing the right mouse button and currently dragging a line
        if (currentEvent.button == 1 && currentBlockNodeGraph.blockNodeToDrawLineFrom != null)
        {
            // Check if over a block node
            BlockNodeSO blockNode = IsMouseOverBlockNode(currentEvent);

            if (blockNode != null)
            {
                // if so set it as a child of the parent block node if it can be added
                if (currentBlockNodeGraph.blockNodeToDrawLineFrom.AddChildBlockNodeIDToBlockNode(blockNode.id))
                {
                    // Set parent ID in child block node
                    blockNode.AddParentBlockNodeIDToBlockNode(currentBlockNodeGraph.blockNodeToDrawLineFrom.id);
                }
            }

            ClearLineDrag();
        }
    }
    /// <summary>
    /// Clear line drag from a block node
    /// </summary>
    private void ClearLineDrag()
    {
        currentBlockNodeGraph.blockNodeToDrawLineFrom = null;
        currentBlockNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }
    /// <summary>
    /// Process mouse drag event
    /// </summary>
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        // process right click drag event - draw line
        if (currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
    }
    private void ProcessKeyboardArrowsEvent(Event currentEvent)
    {
        if (!currentEvent.isKey && isWindowMovingBlocked) return;

        // One key clicked events
        else if(currentEvent.keyCode == KeyCode.LeftArrow)
        {
            ProcessKeyboardArrowsEvent(new Vector2(graphMoveStrenght, 0));
        }     
        else if(currentEvent.keyCode == KeyCode.RightArrow)
        {
            ProcessKeyboardArrowsEvent(new Vector2(-graphMoveStrenght, 0));
        }        
        else if (currentEvent.keyCode == KeyCode.UpArrow)
        {
            ProcessKeyboardArrowsEvent(new Vector2(0,graphMoveStrenght));
        }       
        else if (currentEvent.keyCode == KeyCode.DownArrow)
        {
            ProcessKeyboardArrowsEvent(new Vector2(0,-graphMoveStrenght));
        }
    }
    /// <summary>
    /// Process right mouse drag event  - draw line
    /// </summary>
    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (currentBlockNodeGraph.blockNodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }
    /// <summary>
    /// Process left mouse drag event - drag block node graph
    /// </summary>
    private void ProcessKeyboardArrowsEvent(Vector2 dragDelta)
    {
        graphDrag = dragDelta;

        for (int i = 0; i < currentBlockNodeGraph.blockNodeList.Count; i++)
        {
            currentBlockNodeGraph.blockNodeList[i].DragNode(dragDelta);
        }

        GUI.changed = true;
    }
    /// <summary>
    /// Draw connections in the graph window between block nodes
    /// </summary>
    private void DrawblockConnections()
    {
        // Loop through all block nodes
        foreach (BlockNodeSO blockNode in currentBlockNodeGraph.blockNodeList)
        {
            if (blockNode.childBlockNodeIDList.Count > 0)
            {
                // Loop through child block nodes
                foreach (string childblockNodeID in blockNode.childBlockNodeIDList)
                {
                    // get child block node from dictionary
                    if (currentBlockNodeGraph.blockNodeDictionary.ContainsKey(childblockNodeID))
                    {
                        DrawConnectionLine(blockNode, currentBlockNodeGraph.blockNodeDictionary[childblockNodeID]);

                        GUI.changed = true;
                    }
                }
            }
        }
    }
    /// <summary>
    /// Draw connection line between the parent block node and child block node
    /// </summary>
    private void DrawConnectionLine(BlockNodeSO parentblockNode, BlockNodeSO childblockNode)
    {
        // get line start and end position
        //Vector2 startPosition = parentblockNode.rect.center;
        //Vector2 endPosition = childblockNode.rect.center;
        Vector2 startPosition = parentblockNode.GetBlockRect(parentblockNode.rect).center;
        Vector2 endPosition = childblockNode.GetBlockRect(childblockNode.rect).center;

        // calculate midway point
        Vector2 midPosition = (endPosition + startPosition) / 2f;

        // Vector from start to end position of line
        Vector2 direction = endPosition - startPosition;

        // Calulate normalised perpendicular positions from the mid point
        Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;

        // Calculate mid point offset position for arrow head
        Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;

        // Draw Arrow
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, connectingLineWidth);

        // Draw line
        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, connectingLineWidth);

        GUI.changed = true;
    }
    /// <summary>
    /// Drag connecting line from block node
    /// </summary>
    public void DragConnectingLine(Vector2 delta)
    {
        currentBlockNodeGraph.linePosition += delta;
    }

    /// <summary>
    /// Draw block nodes in the graph window
    /// </summary>
    private void DrawBlockNodes()
    {
        // Loop through all block nodes and draw them
        foreach (BlockNodeSO blockNode in currentBlockNodeGraph.blockNodeList)
        {
            // todo more block nodes 
            if (blockNode.isSelected)
            {
                blockNode.Draw(blockNodeSelectedStyle,titleStyle, blockNodeSelectablePlaceStyle);
            }
            else
            {
                blockNode.Draw(blockNodeStyle, titleStyle, blockNodeSelectablePlaceStyle);
            }
        }

        GUI.changed = true;
    }
    /// <summary>
    /// Selection changed in the inspector
    /// </summary>
    private void InspectorSelectionChanged()
    {
        BlockNodeGraphSO blockNodeGraph = Selection.activeObject as BlockNodeGraphSO;

        if (blockNodeGraph != null)
        {
            currentBlockNodeGraph = blockNodeGraph;
            GUI.changed = true;
        }
    }
}
