using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightNodeType_", menuName = "Scriptable Objects/Lighting/Light Node Type")]
public class LightNodeTypeSO : ScriptableObject
{
    public string lightNodeTypeName;

    public bool displayInBlockGraphEditor = true;

    public bool isStartBlock;

    public bool isStepBlock;

    public bool isEndBlock;

    public bool isNone;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(lightNodeTypeName), lightNodeTypeName);
    }
#endif
    #endregion 
}
