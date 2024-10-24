using System.Collections.Generic;
using DOTSRTS.Utilities.DOTSGrid.Components._2D;
using DOTSRTS.Utilities.DOTSGrid.Data;
using DOTSRTS.Utilities.DOTSGrid.Systems._2D;
using DOTSRTS.Utilities.Mono;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTSGrid.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    public partial class GridBakerSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<SerializableGridData>();
        }

        protected override void OnUpdate()
        {
            var blobAssetStore = World.GetExistingSystemManaged<BakingSystem>().BlobAssetStore;

            foreach (var (serializableGridData, gridGridComponentRef, localTransform) in SystemAPI
                         .Query<SerializableGridData, RefRW<GridComponent>, RefRO<LocalTransform>>())
            {
                var serializableGrid = serializableGridData.Value;
                GenerateGrid(serializableGrid, localTransform.ValueRO.Position);

                // Create a new BlobBuilder for each grid
                using var builder = new BlobBuilder(Allocator.Persistent);
                ref GridAssetBlob gridDataAssetBlob = ref builder.ConstructRoot<GridAssetBlob>();
                var cellsArray = builder.Allocate(ref gridDataAssetBlob.Cells, serializableGrid.cells.Count);

                // Fill in the grid cells
                for (int i = 0; i < cellsArray.Length; i++)
                {
                    ref Cell cell = ref cellsArray[i];
                    var settingCell = serializableGrid.cells[i];
                    if (settingCell == null) continue;

                    cell.GridPosition = settingCell.gridPosition;
                    cell.Type = settingCell.type;
                    cell.WorldPosition = settingCell.worldPosition;
                    cell.Bounds = settingCell.Bounds;

                    // Allocate and set cell corners
                    var cellCorners = builder.Allocate(ref cell.Corners, (int)Corner.Amount);
                    for (int cc = 0; cc < cellCorners.Length; cc++)
                    {
                        cellCorners[cc] = settingCell.corners[cc];
                    }
                }

                // Create the BlobAssetReference
                var gridBlobAssetReference = builder.CreateBlobAssetReference<GridAssetBlob>(Allocator.Persistent);

                // Add Blob to BlobAssetStore
                blobAssetStore.TryAdd(ref gridBlobAssetReference, out var hash);

                // Create and assign GridComponent
                var gridComponent = new GridComponent
                {
                    ID = serializableGrid.ID,
                    InstanceID = gridGridComponentRef.ValueRO.InstanceID,
                    Layer = serializableGrid.Layer,
                    RotationAxis = serializableGrid.RotationAxis,
                    CellSize = serializableGrid.CellSize,
                    Scale = serializableGrid.Scale,
                    HalfScale = serializableGrid.HalfScale,
                    Rotation = serializableGrid.Rotation,
                    GridAsset = gridBlobAssetReference,
                    Bounds = serializableGrid.Bounds,
                    Origin = serializableGrid.origin
                };

                gridGridComponentRef.ValueRW = gridComponent;


            }
        }

        // Method to generate the grid and fill in the grid cells
        private void GenerateGrid(SerializableGrid grid, float3 centerPosition)
        {
            var scale = grid.Scale;
            var halfScale = grid.HalfScale;
            var totalCells = grid.TotalCells;
            var origin = centerPosition - new float3(halfScale.x, 0, halfScale.y);

            // Create NativeArrays to hold output for positions, corners, and corner cells
            NativeArray<float3> worldPositions = new NativeArray<float3>(totalCells, Allocator.TempJob);
            NativeArray<int2> gridPositions = new NativeArray<int2>(totalCells, Allocator.TempJob);
            NativeArray<float3x4> cornersArray = new NativeArray<float3x4>(totalCells, Allocator.TempJob);
            NativeArray<int> cornerIndices = new NativeArray<int>(4, Allocator.TempJob);

            try
            {
                // Job 1: Generate grid positions and world positions
                GridGenerationJobs.GenerateGridJob gridJob = new GridGenerationJobs.GenerateGridJob
                {
                    Scale = grid.Scale,
                    CellSize = grid.CellSize,
                    Origin = origin,
                    WorldPositions = worldPositions,
                    GridPositions = gridPositions,
                    Rotation = grid.Rotation,
                    CenterPosition = centerPosition
                };

                JobHandle gridJobHandle = gridJob.Schedule(totalCells, 64);

                // Job 2: Calculate cell corners after grid generation
                GridGenerationJobs.SetCellCornersJob cornersJob = new GridGenerationJobs.SetCellCornersJob
                {
                    CellSize = grid.CellSize,
                    Rotation = grid.Rotation,
                    WorldPositions = worldPositions,
                    Corners = cornersArray
                };

                JobHandle cornersJobHandle = cornersJob.Schedule(totalCells, 64, gridJobHandle);

                // Job 3: Identify the corner cells of the grid
                GridGenerationJobs.SetGridCornersJob gridCornersJob = new GridGenerationJobs.SetGridCornersJob
                {
                    Rows = scale.y,
                    Columns = scale.x,
                    CornerIndices = cornerIndices
                };

                JobHandle gridCornersJobHandle = gridCornersJob.Schedule(cornersJobHandle);

                // Complete all jobs
                gridCornersJobHandle.Complete();

                // Loop over the grid cells and update or add them
                for (int i = 0; i < totalCells; i++)
                {
                    int2 gridPos = gridPositions[i];

                    // Try to find the existing cell at the grid position
                    var existingCell = grid.cells.Find(c => c.gridPosition.Equals(gridPos));

                    if (existingCell != null)
                    {
                        // Update the existing cell data
                        existingCell.worldPosition = worldPositions[i];
                        existingCell.corners = new List<float3>
                            { cornersArray[i].c0, cornersArray[i].c1, cornersArray[i].c2, cornersArray[i].c3 };
                        existingCell.neighbors = new List<SerializableCell>(); // Update if needed
                        // Update other properties like Type, if necessary
                    }
                    else
                    {
                        // Create a new cell and add it to the list
                        var newCell = new SerializableCell
                        {
                            gridPosition = gridPos,
                            worldPosition = worldPositions[i],
                            neighbors = new List<SerializableCell>(),
                            corners = new List<float3>
                                { cornersArray[i].c0, cornersArray[i].c1, cornersArray[i].c2, cornersArray[i].c3 },
                            // Assign the Type or other properties if needed
                            type = 0
                        };

                        grid.cells.Add(newCell);
                    }
                }

                // Set the grid corners based on the identified indices
                grid.corners.Clear();
                grid.corners.Add(grid.cells[cornerIndices[0]]); // Top-left
                grid.corners.Add(grid.cells[cornerIndices[1]]); // Top-right
                grid.corners.Add(grid.cells[cornerIndices[2]]); // Bottom-left
                grid.corners.Add(grid.cells[cornerIndices[3]]); // Bottom-right
                grid.origin = origin;
            }
            finally
            {
                // Dispose of NativeArrays to prevent memory leaks
                if (worldPositions.IsCreated) worldPositions.Dispose();
                if (gridPositions.IsCreated) gridPositions.Dispose();
                if (cornersArray.IsCreated) cornersArray.Dispose();
                if (cornerIndices.IsCreated) cornerIndices.Dispose();
            }
        }
    }
}
