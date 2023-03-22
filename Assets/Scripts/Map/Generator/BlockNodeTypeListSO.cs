using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockNodeTypeListSO", menuName = "Scriptable Objects/Map/Block Node Type List")]
public class BlockNodeTypeListSO : ScriptableObject
{
    public List<BlockNodeTypeSO> list;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(list), list);
    }
#endif
    #endregion 

}
