using DOTSRTS.Utilities.DOTS.Attributes;
using DOTSRTS.Utilities.DOTS.Editor;
using DOTSRTS.Utilities.DOTSGrid.Data;
using DOTSRTS.Utilities.Mono;
using Drawing;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTSRTS.Utilities.DOTSGrid.Editor
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    [UpdateOnSceneEditor, UpdateOnSceneGUI]
    public partial class Grid2DDataDebugVisualSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<GridComponent>();
        }

        protected override void OnUpdate()
        {
            foreach (var (gridComponent,debugGridData) in SystemAPI.Query<RefRO<GridComponent>, RefRO<DebugGridData>>())
            {
                // Ensure the grid asset is created
                if (!gridComponent.ValueRO.GridAsset.IsCreated)
                    continue;

                ref var cells = ref gridComponent.ValueRO.GridAsset.Value.Cells;
                if (cells.Length == 0)
                    continue;

                // Cache grid properties
                var gridDebugColor = debugGridData.ValueRO.DebugColor.ToColor();
                var totalSize = new float2(gridComponent.ValueRO.Scale.x * gridComponent.ValueRO.CellSize,
                    gridComponent.ValueRO.Scale.y * gridComponent.ValueRO.CellSize);
                var gridRotation = gridComponent.ValueRO.Rotation;
                var cellCounts = gridComponent.ValueRO.Scale;

                Draw.WireGrid(gridComponent.ValueRO.Bounds.Center, gridRotation, cellCounts, totalSize, gridDebugColor);
            }
        }
    }
    
}