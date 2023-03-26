using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine;
using System;

[Serializable]
public class StepBlockNode 
{
    public GameObject stepBlockPrefab;

    #region Editor Code
#if UNITY_EDITOR


    public void DrawStepBlock()
    {
        EditorGUILayout.Space(30);
        EditorGUILayout.LabelField("Prefab: ");
        stepBlockPrefab = (GameObject)EditorGUILayout.ObjectField(stepBlockPrefab, typeof(GameObject), true);

        if (stepBlockPrefab != null)
        {
            if (stepBlockPrefab.TryGetComponent(out Image image))
            {
                EditorGUI.DrawPreviewTexture(new Rect(160, 25, 60, 60), image.mainTexture);
            }
        }

    }

#endif
    #endregion Editor Code
}
