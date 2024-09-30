using DOTSRTS.Utilities.DOTSGrid.Components._2D;
using DOTSRTS.Utilities.DOTSGrid.Data;
using DOTSRTS.Utilities.Mono;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTSGrid.Systems._2D
{
    [WorldSystemFilter(WorldSystemFilterFlags.All)]
    public partial class Grid2DDebugVisualEditorSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<GridComponent>();
        }

        protected override void OnUpdate()
        {
            foreach (var gridComponent in SystemAPI.Query<RefRO<GridComponent>>().WithAll<Grid2d>())
            {
                // Get the BlobAssetReference that contains the grid data
                BlobAssetReference<GridAssetBlob> gridAsset = gridComponent.ValueRO.GridAsset;

                if (gridAsset.IsCreated)
                {
                    // Draw the grid
                    DrawGrid(gridAsset);
                }
            }
        }

        private void DrawGrid(BlobAssetReference<GridAssetBlob> gridAsset)
        {
            // Retrieve grid parameters from the blob asset
            float cellSize = gridAsset.Value.CellSize;
            float halfCellSize = cellSize / 2.0f;
            var gridColor = gridAsset.Value.Color.ToColor();

            // Apply the grid's axis rotation (use the method based on the RotationAxis)
            var gridRotation = ApplyAxisRotation(gridAsset.Value.RotationAxis); // We will use the axis rotation now


            // for (Corner corner = 0; corner < Corner.Amount; corner++)
            // {
                DebugExtensions.DrawWireSphere( gridAsset.Value.GetCorner(Corner.TopLeft).WorldPosition, 2f, Color.red);
           // }

            // Loop through each cell in the grid and draw the grid lines
            for (int i = 0; i < gridAsset.Value.Cells.Length; i++)
            {
                ref var cell = ref gridAsset.Value.Cells[i];
                float3 cellPosition = cell.WorldPosition;

                // Calculate the corners of the cell based on its position and size

                var topLeft = gridAsset.Value.Corners[0].Corners[0];
                var topRight = math.mul(gridRotation, new float3(halfCellSize, 0, halfCellSize)) + cellPosition;
                var bottomLeft = math.mul(gridRotation, new float3(-halfCellSize, 0, -halfCellSize)) + cellPosition;
                var bottomRight = math.mul(gridRotation, new float3(halfCellSize, 0, -halfCellSize)) + cellPosition;

                // Draw lines forming the edges of the grid cell
                Debug.DrawLine(topLeft, topRight, gridColor);
                Debug.DrawLine(topRight, bottomRight, gridColor);
                Debug.DrawLine(bottomRight, bottomLeft, gridColor);
                Debug.DrawLine(bottomLeft, topLeft, gridColor);

                
                
                for (Corner corner = 0; corner < Corner.Amount; corner++)
                {
                    DebugExtensions.DrawWireSphere(cell.GetCorner(corner), gridAsset.Value.CellSize / 10, gridAsset.Value.Color.ToColor());
                }
                // Optionally, draw a point at the center of each cell
            //    DebugExtensions.DrawWireSphere(cell.GetCorner(Corner.TopLeft), 1f, Color.cyan);

                // Draw the corners of each cell
             
                // DebugExtensions.DrawWireSphere(topRight, 0.25f, Color.blue);
                // DebugExtensions.DrawWireSphere(bottomLeft, 0.25f, Color.blue);
                // DebugExtensions.DrawWireSphere(bottomRight, 0.25f, Color.blue);
            }

            for (int i = 0; i < gridAsset.Value.Corners.Length; i++)
            {
                ref var cornerPosition = ref gridAsset.Value.Corners[i].WorldPosition;
                
                // Optionally, draw a point at the grid corners
                DebugExtensions.DrawWireSphere(cornerPosition, 1f, Color.magenta);
            }

            // Draw the grid bounds (if necessary)
            DrawGridBounds(gridAsset);
        }

        // Draw the bounds of the grid using Float3Bounds
        private void DrawGridBounds(BlobAssetReference<GridAssetBlob> gridAsset)
        {
            // Extract the bounds
            var bounds = gridAsset.Value.Bounds;

            // Define the corners of the bounds
            float3 bottomLeft = bounds.Min;
            float3 bottomRight = new float3(bounds.Max.x, 0, bounds.Min.z);
            float3 topLeft = new float3(bounds.Min.x, 0, bounds.Max.z);
            float3 topRight = bounds.Max;

            // Draw the boundary box of the grid using Debug.DrawLine
            Debug.DrawLine(bottomLeft, bottomRight, Color.yellow);
            Debug.DrawLine(bottomRight, topRight, Color.yellow);
            Debug.DrawLine(topRight, topLeft, Color.yellow);
            Debug.DrawLine(topLeft, bottomLeft, Color.yellow);

            // Draw the corners of the grid bounds
            DebugExtensions.DrawWireSphere(bottomLeft, 0.5f, Color.red);
            DebugExtensions.DrawWireSphere(bottomRight, 0.5f, Color.red);
            DebugExtensions.DrawWireSphere(topLeft, 0.5f, Color.red);
            DebugExtensions.DrawWireSphere(topRight, 0.5f, Color.red);

            // Draw the center of the grid
            DebugExtensions.DrawWireSphere(bounds.Center, 1, Color.cyan);
        }

        // Helper method to adjust the rotation axis
        private quaternion ApplyAxisRotation(Axis rotationAxis)
        {
            return rotationAxis switch
            {
                Axis.X => quaternion.Euler(math.radians(90f), 0f, 0f),
                Axis.Y => quaternion.Euler(0f, math.radians(90f), 0f),
                Axis.Z => quaternion.Euler(0f, 0f, math.radians(90f)),
                _ => quaternion.identity,  // Default to identity if none specified
            };
        }
    }
}
