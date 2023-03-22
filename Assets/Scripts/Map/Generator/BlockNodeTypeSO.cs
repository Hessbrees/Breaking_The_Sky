using UnityEngine;

[CreateAssetMenu(fileName = "BlockNodeType_", menuName = "Scriptable Objects/Map/Block Node Type")]
public class BlockNodeTypeSO : ScriptableObject
{
    public string blockNodeTypeName;

    public bool displayInBlockGraphEditor = true;

    public bool isStartBlock;

    public bool isConnectBlock;

    public bool isStepBlock;

    public bool isEndBlock;

    public bool isNone;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(blockNodeTypeName), blockNodeTypeName);
    }
#endif
    #endregion 

}
