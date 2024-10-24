using System.Linq;
using DOTSRTS.Events.Editor;
using DOTSRTS.Utilities.DOTSGrid.Authoring._2D;
using DOTSRTS.Utilities.DOTSGrid.Data;
using DOTSRTS.Utilities.DOTSGrid.Systems._2D;
using DOTSRTS.Utilities.Mono.GameObjects;
using DOTSRTS.Utilities.Mono.Transforms;
using DOTSRTS.Utilities.PrefabsManagement;
using DOTSRTS.Utilities.PrefabsManagement.Authoring;
using DOTSRTS.Utilities.PrefabsManagement.Components;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTSGrid.Editor
{
    public class GridManagerWindow : OdinEditorWindow
    {
        [OnValueChanged("OnUpdateSerializableGrid", true)]
        public SerializableGrid selectedSerializableGrid;

        [OnValueChanged("OnUpdateSerializableCell", true)]
        public SerializableCell serializableCell;

        private Grid2dAuthoring _currentAuthoring;
        private GridData _gridData;

        [MenuItem("Tools/Grid/Selected Cell Inspector")]
        private static void OpenWindow()
        {
            var window = GetWindow<GridManagerWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
            window.Show();
        }

        protected override void OnEnable()
        {
            _gridData = GridData.Instance;
            if (_currentAuthoring == null)
            {
                _currentAuthoring = FindObjectsByType<Grid2dAuthoring>(FindObjectsInactive.Include,FindObjectsSortMode.InstanceID).FirstOrDefault();
            }
            EditorEventManager.EditorEventAggregator.Subscribe<OnGridActiveEvent>(OnGridSelectionChanged);
            EditorEventManager.EditorEventAggregator.Subscribe<OnCellSelectedEvent>(OnCellSelectionChanged);
            base.OnEnable();
        }

        public void OnUpdateSerializableCell()
        {
            UpdatePrefabList();
            UpdateSelectedCell();
            Repaint();
        }

        private void UpdatePrefabList()
        {
            var prefabManager = GetOrCreatePrefabManager();
            if (prefabManager == null) return;

            var prefabList = prefabManager.prefabsList;

            if (serializableCell.SpawnPrefab)
            {
                var prefab = serializableCell.prefabData.prefab;
                if (prefab != null)
                {
                    
                    Debug.Log($"Size : {serializableCell.Bounds.Size}");
                    var bakingPrefab = new BakingPrefabData
                    {
                        id = prefabManager.prefabsList.Count,
                        prefab = prefab,
                        position = serializableCell.worldPosition,
                        rotation = quaternion.identity,
                        scale = GetScaleTransformBasedOnCellBounds(prefab.transform.Scale(),serializableCell.Bounds)
                    };
                    
                    if (!prefabList.Exists(p => p == bakingPrefab))
                    {
                        prefabList.Add(bakingPrefab);
                    }

                    serializableCell.prefabData.prefab = prefab;
                }
                
            }
            else
            {
                var existingPrefab = prefabList.Find(p => p.id == serializableCell.prefabData.id);
                if (existingPrefab != null)
                {
                    prefabList.Remove(existingPrefab);
                }
            }
            _gridData.RefreshBake();
            
        }

        private RequestsForPrefabSpawnAuthoring GetOrCreatePrefabManager()
        {
            if (_currentAuthoring == null) return null;
            var prefabManager = _currentAuthoring.GetComponent<RequestsForPrefabSpawnAuthoring>();
            var needsManager = selectedSerializableGrid.SpawnsPrefabs;

            switch (needsManager)
            {
                case true when prefabManager == null:
                    prefabManager = _currentAuthoring.AddComponent<RequestsForPrefabSpawnAuthoring>();
                    break;
                case false when prefabManager != null:
                    DestroyImmediate(prefabManager);
                    break;
            }
            return prefabManager;
        }

        private void UpdateSelectedCell()
        {
            _gridData.SerializableGrids[selectedSerializableGrid.ID].UpdateCell(serializableCell);
        }

        public void OnUpdateSerializableGrid()
        {
            _gridData.SerializableGrids[selectedSerializableGrid.ID] = selectedSerializableGrid;
            GetOrCreatePrefabManager();
        }

        public void OnGridSelectionChanged(OnGridActiveEvent args)
        {
            selectedSerializableGrid = args.SerializableGrid;
            _currentAuthoring = EditorGridTools.GetGridObjectByInstance(args.GridComponent.InstanceID);
            GetOrCreatePrefabManager();

            GameObjectTools.FocusOnGameObject(_currentAuthoring.gameObject);
            Repaint();
        }

        public void OnCellSelectionChanged(OnCellSelectedEvent args)
        {
            if (selectedSerializableGrid == null) return;
            serializableCell = selectedSerializableGrid.GetCell(args.Cell.GridPosition);
            Repaint();
        }

        protected override void OnImGUI()
        {
            base.OnImGUI();
            if (GUI.changed)
            {
                _gridData.RefreshBake();
            }
        }
        
        public static float GetScaleTransformBasedOnCellBounds(float normalizedScale, BoundsFloat3 cellBounds, float min = 0f)
        {
            // Clamp the normalized scale value between min and 1
            normalizedScale = math.clamp(normalizedScale, min, 1f);

            // Calculate the scale factor based on the cell size and ensure non-negative size
            float3 maxScale = math.abs(cellBounds.Size);

            // Ensure the maxScale is not zero to avoid invalid scaling
            if (math.all(maxScale == float3.zero))
            {
                Debug.LogWarning("Cell bounds size is zero. Returning default scale of 0.");
                return 0f;
            }

            // Compute the new scale by interpolating between zero and the maxScale
            float3 newScale = math.lerp(float3.zero, maxScale, normalizedScale);
            float scale = TransformTools.ConvertLossyScaleToFloat(newScale);
            Debug.Log("Computed Scale: " + scale);

            return scale;
        }

        
        protected override void OnDestroy()
        {
            EditorEventManager.EditorEventAggregator.Unsubscribe<OnGridActiveEvent>(OnGridSelectionChanged);
            EditorEventManager.EditorEventAggregator.Unsubscribe<OnCellSelectedEvent>(OnCellSelectionChanged);
            base.OnDestroy();
        }
    }
}
