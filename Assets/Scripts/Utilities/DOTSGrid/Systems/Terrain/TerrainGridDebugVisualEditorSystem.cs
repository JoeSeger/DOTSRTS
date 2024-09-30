// using DOTSRTS.Utilities.DOTSGrid.Components._2D;
// using DOTSRTS.Utilities.DOTSGrid.Components.Terrain;
// using DOTSRTS.Utilities.DOTSGrid.Data;
// using DOTSRTS.Utilities.Mono;
// using Unity.Entities;
// using Unity.Mathematics;
// using UnityEngine;
//
// namespace DOTSRTS.Utilities.DOTSGrid.Systems.Terrain
// {
//     [WorldSystemFilter(WorldSystemFilterFlags.All)]
//     public partial class TerrainGridDebugVisualEditorSystem : SystemBase
//     {
//         protected override void OnCreate()
//         {
//             base.OnCreate();
//             RequireForUpdate<GridComponent>();
//         }
//
//         protected override void OnUpdate()
//         {
//             // Iterate over all grids in the ECS world that contain a TerrainGrid component and draw them
//             foreach (var gridComponent in SystemAPI.Query<RefRO<GridComponent>>()
//                          .WithAll<TerrainGrid>().WithNone<Grid2d>())
//             {
//                 var gridAsset = gridComponent.ValueRO.GridAsset;
//
//                 if (gridAsset.IsCreated)
//                 {
//                     // Draw the grid using the grid data and bounds information
//                     DrawGrid(gridAsset);
//                 }
//             }
//         }
//
//         private void DrawGrid(BlobAssetReference<GridAssetBlob> gridAsset)
//         {
//             var bounds = gridAsset.Value.Bounds;
//             // Get grid properties from bounds
//             float totalWidth = bounds.Max.x - bounds.Min.x;
//             float totalHeight = bounds.Max.z - bounds.Min.z;
//             float cellSize = gridAsset.Value.CellSize;
//
//             int widthInCells = (int)math.round(totalWidth / cellSize);
//             int heightInCells = (int)math.round(totalHeight / cellSize);
//
//             // Draw grid lines based on bounds and cells
//             for (int row = 0; row < heightInCells; row++)
//             {
//                 for (int col = 0; col < widthInCells; col++)
//                 {
//                     // Get the index of the current cell
//                     int index = row * widthInCells + col;
//                     ref var currentCell = ref gridAsset.Value.Cells[index];
//
//                     float3 currentPosition = currentCell.WorldPosition;
//
//                     // Draw lines to the right
//                     if (col < widthInCells - 1)
//                     {
//                         int rightIndex = index + 1;
//                         ref var rightCell = ref gridAsset.Value.Cells[rightIndex];
//                         float3 rightPosition = rightCell.WorldPosition;
//                         Debug.DrawLine(currentPosition, rightPosition, gridAsset.Value.Color.ToColor());
//                     }
//
//                     // Draw lines downwards
//                     if (row < heightInCells - 1)
//                     {
//                         int belowIndex = index + widthInCells;
//                         ref var belowCell = ref gridAsset.Value.Cells[belowIndex];
//                         float3 belowPosition = belowCell.WorldPosition;
//                         Debug.DrawLine(currentPosition, belowPosition, gridAsset.Value.Color.ToColor());
//                     }
//                 }
//             }
//         }
//     }
// }
