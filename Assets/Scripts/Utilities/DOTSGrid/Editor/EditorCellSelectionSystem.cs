using DOTSRTS.Events.Editor;
using DOTSRTS.Utilities.DOTS.Attributes;
using DOTSRTS.Utilities.DOTS.Editor;
using DOTSRTS.Utilities.DOTSGrid.Components;
using DOTSRTS.Utilities.DOTSGrid.Data;
using DOTSRTS.Utilities.DOTSGrid.Systems._2D;
using Drawing;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTSGrid.Editor
{

    public class OnCellSelectedEvent
    {
        public Cell Cell;
    }
    
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    [UpdateOnSceneEditor, UpdateOnSceneGUI]
    public partial class EditorCellSelectionSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<ActiveGrid>();
            RequireForUpdate<SelectedCellData>();
            
        }
   


        protected override void OnUpdate()
        {
            var selectedCellData = SystemAPI.GetSingleton<SelectedCellData>();

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            

            if (EditorMouseEventManager.LeftButtonPressed)
            {
                foreach (var gridComponent in SystemAPI.Query<RefRO<GridComponent>>())
                {
                    ref var cellsAssetBlob = ref gridComponent.ValueRO.GridAsset.Value;

                    for (int i = 0; i < cellsAssetBlob.Cells.Length; i++)
                    {
                        ref var cell = ref cellsAssetBlob.Cells[i];
                        selectedCellData.CellIsSelected = EditorGridTools.EditorSceneViewIsMouseOverBounds(cell.Bounds);
                        if (!selectedCellData.CellIsSelected) continue;
                        selectedCellData.Bounds = cell.Bounds;
                        selectedCellData.WorldPosition = cell.WorldPosition;
                        selectedCellData.GridID = gridComponent.ValueRO.ID;
                        selectedCellData.GridPosition = cell.GridPosition;
                        selectedCellData.GridInstanceID = gridComponent.ValueRO.InstanceID;
                        
                        EditorEventManager.EditorEventAggregator.Publish(new OnCellSelectedEvent
                        {
                            Cell = cell,
                        });
                        
                  
                        
                    }
                }
            }
            

            SystemAPI.SetSingleton(selectedCellData);
            // 0 is the left mouse button
            ecb.Playback(EntityManager);
            ecb.Dispose();
            // Mark the event as used so it doesn't propagate
        }

   
    }

    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    [UpdateOnSceneEditor, UpdateOnSceneGUI]
    public partial class EditorCellSelectionVisualSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<ActiveGrid>();
            RequireForUpdate<SelectedCellData>();
        }

        protected override void OnUpdate()
        {
            // Check for left mouse button click

            var selectedCellData = SystemAPI.GetSingleton<SelectedCellData>();
            var cellBounds = selectedCellData.Bounds;

            if (!cellBounds.Equals(default))
            {
                Draw.WireBox(cellBounds.Center, cellBounds.Size, Color.red);
            }

            SystemAPI.SetSingleton(selectedCellData);
        }
    }
}