using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightNodeTypeListSO", menuName = "Scriptable Objects/Lighting/Light Node Type List")]
public class LightNodeTypeListSO : ScriptableObject
{
    public List<LightNodeTypeSO> list;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(list), list);
    }
#endif
    #endregion 
}
