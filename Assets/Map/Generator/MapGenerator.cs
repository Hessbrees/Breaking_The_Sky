using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class MapGenerator: EditorWindow
{
    [MenuItem("Map Generator Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<MapGenerator>("Map Generator Graph Editor");
    }
}
