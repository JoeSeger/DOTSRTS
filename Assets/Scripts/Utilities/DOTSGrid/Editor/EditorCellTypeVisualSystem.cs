using System.Collections.Generic;
using DOTSRTS.Utilities.DOTS.Attributes;
using DOTSRTS.Utilities.DOTS.Editor;
using DOTSRTS.Utilities.DOTSGrid.Data;
using DOTSRTS.Utilities.Mono;
using Drawing;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTSGrid.Editor
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    [UpdateOnSceneEditor, UpdateOnSceneGUI]
    public partial class EditorCellTypeVisualSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var (gridComponent, debugGridData, debugGridCellTypeDatas) in SystemAPI.Query<RefRO<GridComponent>, RefRO<DebugGridData>, DynamicBuffer<DebugGridCellTypeData>>())
            {
                ref var cellsAssetBlob = ref gridComponent.ValueRO.GridAsset.Value;

                for (int i = 0; i < cellsAssetBlob.Cells.Length; i++)
                {
                    ref var cell = ref cellsAssetBlob.Cells[i];

                    // Get all matching colors for the flags present in cell.Type
                    var matchingColors = new List<Color>();
                    foreach (var debugData in debugGridCellTypeDatas)
                    {
                        // Check if this specific CellType flag is present in both cell.Type and ViewingCellTypes
                        if ((cell.Type & debugGridData.ValueRO.ViewingCellTypes & debugData.CellType) == debugData.CellType)
                        {
                            matchingColors.Add(debugData.Color.ToColor());
                        }
                    }

                    // Skip rendering if no flags matched
                    if (matchingColors.Count == 0) continue;

                    var cellBounds = cell.Bounds;
                    float3 cellCenter = cellBounds.Center;
                    float3 cellSize = cellBounds.Size;

                    // Divide the cell evenly among the matched colors
                    float sliceWidth = cellSize.x / matchingColors.Count;

                    for (int j = 0; j < matchingColors.Count; j++)
                    {
                        float3 slicePosition = new float3(
                            cellCenter.x - (cellSize.x / 2) + (j * sliceWidth) + (sliceWidth / 2),
                            cellCenter.y,
                            cellCenter.z
                        );

                        var sliceSize = new float3(sliceWidth, cellSize.y, cellSize.z);
                        Draw.SolidBox(slicePosition, sliceSize, matchingColors[j]);
                    }
                }
            }
        }
    }
}
