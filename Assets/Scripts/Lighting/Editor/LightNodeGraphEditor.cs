using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class LightNodeGraphEditor : EditorWindow
{
    private GUIStyle lightNodeStyle;
    private GUIStyle lightNodeSelectedStyle;
    private GUIStyle lightNodeSelectablePlaceStyle;
    private GUIStyle titleStyle;
    private static LightNodeGraphSO currentLightNodeGraph;
    // Node layout values
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    private Vector2 graphOffset;
    private Vector2 graphDrag;

    private LightNodeSO currentLightNode = null;
    private LightNodeTypeListSO lightNodeTypeList;

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

    [MenuItem("Lighting Graph Editor", menuItem = "Window/Light Editor/Lighting Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<LightNodeGraphEditor>("Lighting Graph Editor");
    }

    [OnOpenAsset(0)]  // Need the namespace UnityEditor.Callbacks
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        LightNodeGraphSO lightNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as LightNodeGraphSO;

        if (lightNodeGraph != null)
        {
            OpenWindow();

            currentLightNodeGraph = lightNodeGraph;

            return true;
        }
        return false;
    }

    private void OnEnable()
    {
        // Subscribe to the inspector selection changed event
        Selection.selectionChanged += InspectorSelectionChanged;

        // Define node layout style
        lightNodeStyle = new GUIStyle();
        lightNodeStyle.normal.textColor = Color.white;
        lightNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        lightNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        lightNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        // Define selected node style
        lightNodeSelectedStyle = new GUIStyle();
        lightNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        lightNodeSelectedStyle.normal.textColor = Color.white;
        lightNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        lightNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        Texture2D selectablePlaceTexture = Texture2D.grayTexture;

        lightNodeSelectablePlaceStyle = new GUIStyle();
        lightNodeSelectablePlaceStyle.normal.background = selectablePlaceTexture;

        titleStyle = new GUIStyle();
        titleStyle.normal.textColor = Color.green;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.fontSize = 18;

        // Load Light node types
        lightNodeTypeList = GameResources.Instance.lightNodeTypeList;
    }
    private void OnGUI()
    {

        // If a scriptable object of type LightNodeGraphSO has been selected then process
        if (currentLightNodeGraph != null)
        {
            VerticalControlers();

            // Draw Grid
            DrawBackgroundGrid(gridSmall, 0.2f, Color.gray);
            DrawBackgroundGrid(gridLarge, 0.3f, Color.gray);

            // Draw line if being dragged
            DrawDraggedLine();

            // Process Events
            ProcessEvents(Event.current);

            // Draw Connections Between light Nodes
            DrawNodesConnections();

            // Draw light Nodes
            DrawLightNodes();

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
    /// Draw a background grid for the light node graph editor
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
        if (currentLightNodeGraph.linePosition != Vector2.zero)
        {
            //Draw line from node to line position
            Handles.DrawBezier(currentLightNodeGraph.lightNodeToDrawLineFrom.rect.center, currentLightNodeGraph.linePosition, currentLightNodeGraph.lightNodeToDrawLineFrom.rect.center, currentLightNodeGraph.linePosition, Color.white, null, connectingLineWidth);
        }
    }

    private void ProcessEvents(Event currentEvent)
    {
        // Reset graph drag
        graphDrag = Vector2.zero;

        // Get light that mouse is over if it's null or not currently being dragged
        if (currentLightNode == null || currentLightNode.isLeftClickDragging == false)
        {
            currentLightNode = IsMouseOverLightNode(currentEvent);
        }

        // if mouse isn't over a light node
        if (currentLightNode == null || currentLightNodeGraph.lightNodeToDrawLineFrom != null)
        {
            ProcesslightNodeGraphEvents(currentEvent);
        }
        // else process light node events
        else
        {
            // process light node events
            currentLightNode.ProcessEvents(currentEvent);
        }

        ProcessKeyboardArrowsEvent(currentEvent);

    }
    /// <summary>
    ///  Check to see to mouse is over a light node - if so then return the light node else return null
    /// </summary>
    private LightNodeSO IsMouseOverLightNode(Event currentEvent)
    {
        for (int i = currentLightNodeGraph.lightNodeList.Count - 1; i >= 0; i--)
        {
            if (currentLightNodeGraph.lightNodeList[i].selectableRect.Contains(currentEvent.mousePosition))
            {
                return currentLightNodeGraph.lightNodeList[i];
            }
        }

        return null;
    }
    private void ProcesslightNodeGraphEvents(Event currentEvent)
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
            ClearAllSelectedLightNodes();
        }
    }
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Create Light Node"), false, CreateLightNode, mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Select All Light Nodes"), false, SelectAllBlockNodes);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Selected Light Node Links"), false, DeleteSelectedLightNodeLinks);
        menu.AddItem(new GUIContent("Delete Selected Light Nodes"), false, DeleteSelectedBlockNodes);
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

        foreach (LightNodeSO lightNode in currentLightNodeGraph.lightNodeList)
        {
            lightNode.isNodesSelectionBlocked = isNodesSelectionBlocked;
        }

        return isNodesSelectionBlocked;
    }
    private bool BlockNodesPosition()
    {
        isNodesPositionBlocked = !isNodesPositionBlocked;

        foreach (LightNodeSO lightNode in currentLightNodeGraph.lightNodeList)
        {
            lightNode.isNodesPositionBlocked = isNodesPositionBlocked;
        }

        return isNodesPositionBlocked;
    }

    /// <summary>
    /// Create a light node at the mouse position
    /// </summary>
    /// <param name="mousePositionObject"></param>
    private void CreateLightNode(object mousePositionObject)
    {
        // If current node graph empty then add entrance light node first
        if (currentLightNodeGraph.lightNodeList.Count == 0)
        {
            CreateLightNode(new Vector2(200f, 200f), lightNodeTypeList.list.Find(x => x.isStartBlock));

            CreateLightNode(new Vector2(1000f, 200f), lightNodeTypeList.list.Find(x => x.isEndBlock));

            LightNodeSO startBlock = null;
            LightNodeSO endBlock = null;

            foreach (var lightNode in currentLightNodeGraph.lightNodeList)
            {
                if(lightNode.lightNodeType.isStartBlock) startBlock = lightNode;
                
                if(lightNode.lightNodeType.isEndBlock) endBlock = lightNode;
            }

            CreateConnectionLineToStartNode(startBlock,endBlock);
        }

        CreateLightNode(mousePositionObject, lightNodeTypeList.list.Find(x => x.isNone));
    }

    private void CreateLightNode(object mousePositionObject, LightNodeTypeSO lightNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        // create light node scriptable object asset
        LightNodeSO lightNode = ScriptableObject.CreateInstance<LightNodeSO>();

        // add light node to current light node graph light node list
        currentLightNodeGraph.lightNodeList.Add(lightNode);

        // set light node values

        lightNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentLightNodeGraph, lightNodeType);

        // add light node to light node graph scriptable object asset database
        AssetDatabase.AddObjectToAsset(lightNode, currentLightNodeGraph);

        AssetDatabase.SaveAssets();

        // Refresh graph node dictionary
        currentLightNodeGraph.OnValidate();

    }
    private void CreateConnectionLineToStartNode(LightNodeSO childLightNode, LightNodeSO parentLightNode)
    {
        parentLightNode.AddChildLightNodeIDToLightNode(childLightNode.id);

        childLightNode.AddParentLightNodeIDToLightNode(parentLightNode.id);
    }

    /// <summary>
    /// Delete selected light nodes
    /// </summary>
    private void DeleteSelectedBlockNodes()
    {
        Queue<LightNodeSO> lightNodeDeletionQueue = new Queue<LightNodeSO>();

        // Loop through all nodes
        foreach (LightNodeSO lightNode in currentLightNodeGraph.lightNodeList)
        {
            if (lightNode.isSelected)
            {
                lightNodeDeletionQueue.Enqueue(lightNode);

                // iterate through child light nodes ids
                foreach (string childBlockNodeID in lightNode.childLightNodeIDList)
                {
                    // Retrieve child light node
                    LightNodeSO childBlockNode = currentLightNodeGraph.GetLightNode(childBlockNodeID);

                    if (childBlockNode != null)
                    {
                        // Remove parentID from child light node
                        childBlockNode.RemoveParentLightNodeIDFromLightNode(lightNode.id);
                    }
                }

                // Iterate through parent light node ids
                foreach (string parentlightNodeID in lightNode.parentLightNodeIDList)
                {
                    // Retrieve parent node
                    LightNodeSO parentlightNode = currentLightNodeGraph.GetLightNode(parentlightNodeID);

                    if (parentlightNode != null)
                    {
                        // Remove childID from parent node
                        parentlightNode.RemoveChildLightNodeIDFromLightNode(lightNode.id);
                    }
                }
            }
        }

        // Delete queued light nodes
        while (lightNodeDeletionQueue.Count > 0)
        {
            // Get light node from queue
            LightNodeSO lightNodeToDelete = lightNodeDeletionQueue.Dequeue();

            // Remove node from dictionary
            currentLightNodeGraph.lightNodeDictionary.Remove(lightNodeToDelete.id);

            // Remove node from list
            currentLightNodeGraph.lightNodeList.Remove(lightNodeToDelete);

            // Remove node from Asset database
            DestroyImmediate(lightNodeToDelete, true);

            // Save asset database
            AssetDatabase.SaveAssets();

        }
    }

    /// <summary>
    /// Delete the links between the selected light nodes
    /// </summary>
    private void DeleteSelectedLightNodeLinks()
    {
        // Iterate through all light nodes
        foreach (LightNodeSO lightNode in currentLightNodeGraph.lightNodeList)
        {
            if (lightNode.isSelected && lightNode.childLightNodeIDList.Count > 0)
            {
                for (int i = lightNode.childLightNodeIDList.Count - 1; i >= 0; i--)
                {
                    // Get child light node
                    LightNodeSO childlightNode = currentLightNodeGraph.GetLightNode(lightNode.childLightNodeIDList[i]);

                    // If the child light node is selected
                    if (childlightNode != null && childlightNode.isSelected)
                    {
                        // Remove childID from parent light node
                        lightNode.RemoveChildLightNodeIDFromLightNode(childlightNode.id);

                        // Remove parentID from child light node
                        childlightNode.RemoveParentLightNodeIDFromLightNode(lightNode.id);
                    }
                }
            }
        }

        // Clear all selected light nodes
        ClearAllSelectedLightNodes();
    }

    /// <summary>
    /// Clear selection from all light nodes
    /// </summary>
    private void ClearAllSelectedLightNodes()
    {
        foreach (LightNodeSO lightNode in currentLightNodeGraph.lightNodeList)
        {
            if (lightNode.isSelected)
            {
                lightNode.isSelected = false;

                GUI.changed = true;
            }
        }
    }

    /// <summary>
    /// Select all light nodes
    /// </summary>
    private void SelectAllBlockNodes()
    {
        foreach (LightNodeSO lightNode in currentLightNodeGraph.lightNodeList)
        {
            lightNode.isSelected = true;
        }
        GUI.changed = true;
    }

    /// <summary>
    /// Process mouse up events
    /// </summary>
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        // if releasing the right mouse button and currently dragging a line
        if (currentEvent.button == 1 && currentLightNodeGraph.lightNodeToDrawLineFrom != null)
        {
            // Check if over a light node
            LightNodeSO lightNode = IsMouseOverLightNode(currentEvent);

            if (lightNode != null)
            {
                // if so set it as a child of the parent light node if it can be added
                if (currentLightNodeGraph.lightNodeToDrawLineFrom.AddChildLightNodeIDToLightNode(lightNode.id))
                {
                    // Set parent ID in child light node
                    lightNode.AddParentLightNodeIDToLightNode(currentLightNodeGraph.lightNodeToDrawLineFrom.id);
                }
            }

            ClearLineDrag();
        }
    }
    /// <summary>
    /// Clear line drag from a light node
    /// </summary>
    private void ClearLineDrag()
    {
        currentLightNodeGraph.lightNodeToDrawLineFrom = null;
        currentLightNodeGraph.linePosition = Vector2.zero;
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
        else if (currentEvent.keyCode == KeyCode.LeftArrow)
        {
            ProcessKeyboardArrowsEvent(new Vector2(graphMoveStrenght, 0));
        }
        else if (currentEvent.keyCode == KeyCode.RightArrow)
        {
            ProcessKeyboardArrowsEvent(new Vector2(-graphMoveStrenght, 0));
        }
        else if (currentEvent.keyCode == KeyCode.UpArrow)
        {
            ProcessKeyboardArrowsEvent(new Vector2(0, graphMoveStrenght));
        }
        else if (currentEvent.keyCode == KeyCode.DownArrow)
        {
            ProcessKeyboardArrowsEvent(new Vector2(0, -graphMoveStrenght));
        }
    }
    /// <summary>
    /// Process right mouse drag event  - draw line
    /// </summary>
    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (currentLightNodeGraph.lightNodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }
    /// <summary>
    /// Process left mouse drag event - drag light node graph
    /// </summary>
    private void ProcessKeyboardArrowsEvent(Vector2 dragDelta)
    {
        graphDrag = dragDelta;

        for (int i = 0; i < currentLightNodeGraph.lightNodeList.Count; i++)
        {
            currentLightNodeGraph.lightNodeList[i].DragNode(dragDelta);
        }

        GUI.changed = true;
    }
    /// <summary>
    /// Draw connections in the graph window between light nodes
    /// </summary>
    private void DrawNodesConnections()
    {
        // Loop through all light nodes
        foreach (LightNodeSO lightNode in currentLightNodeGraph.lightNodeList)
        {
            if (lightNode.childLightNodeIDList.Count > 0)
            {
                // Loop through child light nodes
                foreach (string childlightNodeID in lightNode.childLightNodeIDList)
                {
                    // get child light node from dictionary
                    if (currentLightNodeGraph.lightNodeDictionary.ContainsKey(childlightNodeID))
                    {
                        DrawConnectionLine(lightNode, currentLightNodeGraph.lightNodeDictionary[childlightNodeID]);

                        GUI.changed = true;
                    }
                }
            }
        }
    }
    /// <summary>
    /// Draw connection line between the parent light node and child light node
    /// </summary>
    private void DrawConnectionLine(LightNodeSO parentLightNode, LightNodeSO childLightNode)
    {
        // get line start and end position
        Vector2 startPosition = parentLightNode.GetBlockRect(parentLightNode.rect).center;
        Vector2 endPosition = childLightNode.GetBlockRect(childLightNode.rect).center;

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
    /// Drag connecting line from light node
    /// </summary>
    public void DragConnectingLine(Vector2 delta)
    {
        currentLightNodeGraph.linePosition += delta;
    }

    /// <summary>
    /// Draw light nodes in the graph window
    /// </summary>
    private void DrawLightNodes()
    {
        // Loop through all light nodes and draw them
        foreach (LightNodeSO lightNode in currentLightNodeGraph.lightNodeList)
        {
            // todo more light nodes 
            if (lightNode.isSelected)
            {
                lightNode.Draw(lightNodeSelectedStyle, titleStyle, lightNodeSelectablePlaceStyle);
            }
            else
            {
                lightNode.Draw(lightNodeStyle, titleStyle, lightNodeSelectablePlaceStyle);
            }
        }

        GUI.changed = true;
    }
    /// <summary>
    /// Selection changed in the inspector
    /// </summary>
    private void InspectorSelectionChanged()
    {
        LightNodeGraphSO lightNodeGraph = Selection.activeObject as LightNodeGraphSO;

        if (lightNodeGraph != null)
        {
            currentLightNodeGraph = lightNodeGraph;
            GUI.changed = true;
        }
    }
}
