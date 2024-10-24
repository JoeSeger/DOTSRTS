using System.Linq;
using DOTSRTS.Events.Editor;
using DOTSRTS.Utilities.DOTSGrid.Authoring._2D;
using DOTSRTS.Utilities.DOTSGrid.Data;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTSGrid.Editor
{
    
    [InitializeOnLoad]
    public static class EditorGridTools
    {
        private static Grid2dAuthoring[] _allGrids;
        static EditorGridTools()
        {
            RefreshGrids();
            EditorEventManager.EditorEventAggregator.Subscribe<EditorOnHierarchyChangedEvent>(OnHHierarchyChanged);
        }
        private static void OnHHierarchyChanged(EditorOnHierarchyChangedEvent onHierarchyChangedEvent)
        {
            RefreshGrids();
        }

        public static Grid2dAuthoring GetGridObjectByInstance(int instanceID) => 
            _allGrids.FirstOrDefault(grid => grid.InstanceID == instanceID);
     
        public static Grid2dAuthoring GetGridObjectByID(int id) => _allGrids.FirstOrDefault(grid => grid.ID == id);

        private static void RefreshGrids()
        {
            _allGrids = Object.FindObjectsByType<Grid2dAuthoring>(FindObjectsInactive.Include,
                FindObjectsSortMode.InstanceID);
            
        }
        public static bool EditorSceneViewIsMouseOverBounds(BoundsFloat3 bounds,float2 mouseScreenPosition, out float3 distance)
        {
            return bounds.IsMouseOverBounds(mouseScreenPosition, out distance);
        }
        public static bool EditorSceneViewIsMouseOverBounds(BoundsFloat3 bounds,float2 mouseScreenPosition)
        {
            return bounds.IsMouseOverBounds(mouseScreenPosition);
        }

        public static bool EditorSceneViewIsMouseOverBounds(BoundsFloat3 bounds)
        {
            return bounds.IsMouseOverBounds(EditorMouseEventManager.ScreenPosition);
        }

    }
}