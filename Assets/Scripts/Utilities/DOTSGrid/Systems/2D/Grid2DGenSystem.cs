using DOTSRTS.Utilities.DOTSGrid.Components._2D;
using DOTSRTS.Utilities.DOTSGrid.Data;
using DOTSRTS.Utilities.Mono;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTSGrid.Systems._2D
{
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    public partial class Grid2DGenSystem : SystemBase
    {
        private BlobAssetStore _blobAssetStore;

        protected override void OnCreate()
        {
            base.OnCreate();
            _blobAssetStore = new BlobAssetStore();
            RequireForUpdate<Generate2DGridRequest>();
        }

        protected override void OnDestroy()
        {
            if (_blobAssetStore.IsCreated) _blobAssetStore.Dispose();
            base.OnDestroy();
        }

        protected override void OnUpdate()
        {
            var generateGridJob = new Generate2DGridJob();
            Dependency = generateGridJob.Schedule(Dependency);
        }

        [BurstCompile]
        public partial struct Generate2DGridJob : IJobEntity
        {
            public void Execute(Entity entity, ref Generate2DGridRequest request, ref GridComponent gridComponent,
                ref LocalTransform localTransform)
            {
                var builder = new BlobBuilder(Allocator.Temp);

                // Construct the root of the BlobAsset
                ref GridAssetBlob gridAsset = ref builder.ConstructRoot<GridAssetBlob>();

                // Set grid properties
                gridAsset.Color = request.DebugColor;
                gridAsset.RotationAxis = request.RotationAxis; // Store only the rotation axis
                gridAsset.CellSize = request.CellSize;

                // Calculate half sizes to help center the grid
                float halfWidth = (request.Columns * gridAsset.CellSize) / 2f;
                float halfHeight = (request.Rows * gridAsset.CellSize) / 2f;

                // Use the local transform's position as the center
                float3 centerPosition = localTransform.Position;
                float3 gridOrigin = centerPosition - new float3(halfWidth, 0, halfHeight);

                // Allocate the BlobArray for the cells based on the Columns and Rows
                var blobCells = builder.Allocate(ref gridAsset.Cells, request.Columns * request.Rows);

                // Step 1: Loop through the grid and populate cell data
                for (int row = 0; row < request.Rows; row++)
                {
                    for (int col = 0; col < request.Columns; col++)
                    {
                        int index = row * request.Columns + col;

                        // Calculate local position for this cell
                        float3 localPosition = new float3(col * gridAsset.CellSize, 0, row * gridAsset.CellSize);

                        // Calculate world position using the origin and local position
                        float3 worldPosition = gridOrigin + localPosition;

                        // Apply grid rotation based on the axis
                        worldPosition = ApplyAxisRotation(gridAsset.RotationAxis, worldPosition - centerPosition) +
                                        centerPosition;

                        // Set cell data
                        ref var cell = ref blobCells[index];
                        cell.GridPosition = new int2(col, row);
                        cell.WorldPosition = worldPosition;
                        cell.Type = 0; // Default type

                        // Allocate and set neighbors (up, down, left, right)
                        var blobNeighbors = builder.Allocate(ref cell.Neighbors, 4);
                        blobNeighbors[0] = row > 0 ? (row - 1) * request.Columns + col : -1; // Top
                        blobNeighbors[1] = row < request.Rows - 1 ? (row + 1) * request.Columns + col : -1; // Bottom
                        blobNeighbors[2] = col > 0 ? row * request.Columns + (col - 1) : -1; // Left
                        blobNeighbors[3] = col < request.Columns - 1 ? row * request.Columns + (col + 1) : -1; // Right

                        
                        // Allocate and set cell corners
                        var cellCorners = builder.Allocate(ref cell.Corners, (int)Corner.Amount);
                        SetCellCorners( ref cellCorners, worldPosition, gridAsset.CellSize,request.RotationAxis);
                    }
                }

               


                var blobGridCorners = builder.Allocate(ref gridAsset.Corners, (int)Corner.Amount);
                StoreGridCorners(ref gridAsset, ref blobGridCorners, ref blobCells, request.Columns, request.Rows);
                
                
                // Create the BlobAssetReference
                var gridBlob = builder.CreateBlobAssetReference<GridAssetBlob>(Allocator.Persistent);
                gridComponent.GridAsset = gridBlob;

                
                gridComponent.GridAsset = gridBlob;

                // Dispose of the builder to free memory
                builder.Dispose();
            }

            
            private static void SetCellCorners(ref BlobBuilderArray<float3> cellCorners, float3 worldPosition, float cellSize, Axis rotationAxis)
            {
                float halfCellSize = cellSize / 2f;

                // Apply the grid's rotation to the corners
                var gridRotation = ApplyAxisRotation(rotationAxis);

                // Calculate the corners of the cell relative to the world position
                cellCorners[(int)Corner.TopLeft] = math.mul(gridRotation, new float3(-halfCellSize, 0, -halfCellSize)) + worldPosition;
                cellCorners[(int)Corner.TopRight] = math.mul(gridRotation, new float3(halfCellSize, 0, -halfCellSize)) + worldPosition;
                cellCorners[(int)Corner.BottomLeft] = math.mul(gridRotation, new float3(-halfCellSize, 0, halfCellSize)) + worldPosition;
                cellCorners[(int)Corner.BottomRight] = math.mul(gridRotation, new float3(halfCellSize, 0, halfCellSize)) + worldPosition;
            }

            private static void StoreGridCorners(ref GridAssetBlob gridAsset, ref BlobBuilderArray<Cell> blobCorners,
                ref BlobBuilderArray<Cell> gridCells, int columns, int rows)
            {
                int topLeftIndex = 0; // The cell at the bottom-left (row 0, col 0)
                int topRightIndex = columns - 1; // The cell at the bottom-right (row 0, col columns - 1)
                int bottomLeftIndex = (rows - 1) * columns; // The cell at the top-left (row rows - 1, col 0)
                int bottomRightIndex = (rows * columns) - 1;
              
                // Get the corners of the respective cells
                ref var bottomLeftCell = ref gridCells[bottomLeftIndex];
                ref var bottomRightCell = ref gridCells[bottomRightIndex];
                ref var topLeftCell = ref gridCells[topLeftIndex];
                ref var topRightCell = ref gridCells[topRightIndex];
                
                
                Debug.Log($"blobCorners :{blobCorners.Length}");
                // Store the appropriate corners of each outer cell as the grid's corners
                blobCorners[(int)Corner.TopLeft] = topLeftCell;
                blobCorners[(int)Corner.TopRight] = topRightCell;
                blobCorners[(int)Corner.BottomLeft] = bottomLeftCell;
                blobCorners[(int)Corner.BottomRight] = bottomRightCell;
                

                SetGridBounds(ref gridAsset, blobCorners);
            }


            public static quaternion ApplyAxisRotation(Axis rotationAxis)
            {
                return rotationAxis switch
                {
                    Axis.X => quaternion.Euler(math.radians(90f), 0f, 0f),
                    Axis.Y => quaternion.Euler(0f, math.radians(90f), 0f),
                    Axis.Z => quaternion.Euler(0f, 0f, math.radians(90f)),
                    _ => quaternion.identity, // Default to identity if none specified
                };
            }

            private static float3 ApplyAxisRotation(Axis rotationAxis, float3 position)
            {
                switch (rotationAxis)
                {
                    case Axis.X:
                        return math.mul(quaternion.Euler(math.radians(90f), 0f, 0f), position);
                    case Axis.Y:
                        return math.mul(quaternion.Euler(0f, math.radians(90f), 0f), position);
                    case Axis.Z:
                        return math.mul(quaternion.Euler(0f, 0f, math.radians(90f)), position);
                    default:
                        return position; // No rotation applied
                }
            }

// Adjust bounds to encompass the entire grid using Float3Bounds
            private static void SetGridBounds(ref GridAssetBlob gridAsset, BlobBuilderArray<Cell> gridCornerCells)
            {
                // Use Float3Bounds to calculate the grid bounds


                Float3Bounds bounds = new Float3Bounds(gridAsset.Bounds.Center, float3.zero);
                var gridRotation = ApplyAxisRotation(gridAsset.RotationAxis);
                float halfSize = gridAsset.CellSize / 2f;
                var topLeft = math.mul(gridRotation, new float3(-halfSize, 0, -halfSize)) +
                              gridCornerCells[(int)Corner.TopLeft].WorldPosition; // Top-left
                var topRight = math.mul(gridRotation, new float3(halfSize, 0, -halfSize)) +
                               gridCornerCells[(int)Corner.TopRight].WorldPosition; // Top-Right
                var bottomLeft = math.mul(gridRotation, new float3(-halfSize, 0, halfSize)) +
                                 gridCornerCells[(int)Corner.BottomLeft].WorldPosition; // Bottom-Left
                var bottomRight = math.mul(gridRotation, new float3(halfSize, 0, halfSize)) +
                                  gridCornerCells[(int)Corner.BottomRight].WorldPosition; // Bottom-Right

                bounds.Encapsulate(topLeft);
                bounds.Encapsulate(topRight);
                bounds.Encapsulate(bottomLeft);
                bounds.Encapsulate(bottomRight);
                gridAsset.Bounds = bounds;

                // Debug log for bounds
                Debug.Log(
                    $"Bounds Min: {gridAsset.Bounds.Min}, Max: {gridAsset.Bounds.Max}, Center: {gridAsset.Bounds.Center}");
            }
        }
    }
}