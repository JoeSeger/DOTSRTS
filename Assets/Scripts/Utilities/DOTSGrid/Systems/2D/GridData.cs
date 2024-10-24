#if UNITY_EDITOR
using System.Collections.Generic;
using DOTSRTS.SceneManagement.Mono;
using DOTSRTS.Utilities.DOTSGrid.Data;
using DOTSRTS.Utilities.ScriptableObjects;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


namespace DOTSRTS.Utilities.DOTSGrid.Systems._2D
{
    
   
    [CreateAssetMenu(fileName = "GridData", menuName = "GridData", order = 0)]
    public class GridData : SerializedSingletonScriptableObject<GridData>
    {
        public List<SerializableGrid> SerializableGrids => serializableGrids;
        [SerializeField,OnValueChanged(nameof(RefreshBake),true)]
        private List<SerializableGrid> serializableGrids = new();

        
        
        public void RefreshBake()
        {
            for (var index = 0; index < serializableGrids.Count; index++)
            {
                var grid = serializableGrids[index];
                grid.ID = index;
            }
            EditorUtility.SetDirty(this);
            SubSceneManagerTools.EditorRefresh();
            
        }
    }
}
#endif