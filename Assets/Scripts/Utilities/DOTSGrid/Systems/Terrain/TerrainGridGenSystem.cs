// using DOTSRTS.Utilities.DOTSGrid.Components.Terrain;
// using DOTSRTS.Utilities.DOTSGrid.Data;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
// using UnityEngine;
//
// namespace DOTSRTS.Utilities.DOTSGrid.Systems.Terrain
// {
//     [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)] // Ensure it's only part of the baking system
//     public partial class TerrainGridGenSystem : SystemBase
//     {
//         private EntityCommandBufferSystem _ecbSystem;
//         private BlobAssetStore _blobAssetStore; // Declare BlobAssetStore
//         private NativeArray<BlobAssetReference<GridAssetBlob>> _gridBlobs;
//
//         protected override void OnCreate()
//         {
//             base.OnCreate();
//
//             // Initialize the EntityCommandBufferSystem
//             _ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
//
//             // Initialize the BlobAssetStore (with a capacity of 10, adjust as needed)
//             _blobAssetStore = new BlobAssetStore(10);
//
//             RequireForUpdate<GenerateTerrainGridRequest>();
//         }
//
//         protected override void OnDestroy()
//         {
//             // Dispose of the BlobAssetStore to free up memory
//             if (_blobAssetStore.IsCreated)
//             {
//                 _blobAssetStore.Dispose();
//             }
//
//             if (_gridBlobs.IsCreated)
//             {
//                 _gridBlobs.Dispose();
//             }
//
//             base.OnDestroy();
//         }
//
//         protected override void OnUpdate()
//         {
//             // Check if the BlobAssetStore is properly initialized
//             if (!_blobAssetStore.IsCreated)
//             {
//                 Debug.LogError("BlobAssetStore is not initialized!");
//                 return;
//             }
//
//             var ecb = _ecbSystem.CreateCommandBuffer();
//             var blobAssetStore = _blobAssetStore; // Capture the BlobAssetStore for main thread storage
//
//             // Allocate NativeArray to hold the grid blobs (processed outside job)
//             if (_gridBlobs.IsCreated)
//                 _gridBlobs.Dispose(); // Clear the previous run
//             _gridBlobs = new NativeArray<BlobAssetReference<GridAssetBlob>>(1, Allocator.TempJob);
//
//             // Schedule the job to create the GridAssetBlob
//             var generateGridJob = new GenerateTerrainGridJob
//             {
//                 GridBlob = _gridBlobs
//             };
//             Dependency = generateGridJob.Schedule(Dependency);
//
//             // Add the JobHandle for the ECB
//             _ecbSystem.AddJobHandleForProducer(Dependency);
//
//             Dependency.Complete(); // Ensure the job is complete before continuing
//
//             // Process entities and store blobs in BlobAssetStore after job completion
//             Entities.WithoutBurst().ForEach((Entity entity, in GenerateTerrainGridRequest request) =>
//             {
//                 // Generate a unique key for the grid asset using Hash128
//                 var gridAssetKey =
//                     new Unity.Entities.Hash128((uint)request.WidthInCells, (uint)request.HeightInCells, 0, 0);
//
//                 // Try to get the GridAssetBlob from the BlobAssetStore
//                 if (!blobAssetStore.TryGet(gridAssetKey, out BlobAssetReference<GridAssetBlob> gridBlob))
//                 {
//                     // Store the new GridAssetBlob in the BlobAssetStore
//                     gridBlob = _gridBlobs[0];
//                     if (!blobAssetStore.TryAdd(gridAssetKey, ref gridBlob))
//                     {
//                         Debug.LogWarning("Failed to add the GridAssetBlob to the BlobAssetStore after baking");
//                     }
//                 }
//          
//                 var gridComponent = EntityManager.GetComponentData<GridComponent>(entity);
//                 gridComponent.GridAsset = gridBlob;
//                 
//                 // Add or update the GridComponent on the entity
//                 EntityManager.SetComponentData(entity, gridComponent);
//             }).Run(); // Run synchronously without Burst
//         }
//
//         [BurstCompile]
//         public partial struct GenerateTerrainGridJob : IJobEntity
//         {
//             public NativeArray<BlobAssetReference<GridAssetBlob>> GridBlob;
//
//             public void Execute(Entity entity, ref GenerateTerrainGridRequest request)
//             {
//                 var builder = new BlobBuilder(Allocator.Temp);
//                 ref GridAssetBlob gridAsset = ref builder.ConstructRoot<GridAssetBlob>();
//
//                 // Set grid properties
//                 gridAsset.Bounds.Center = new float3(request.TerrainSize.x / 2f, 0, request.TerrainSize.z / 2f);
//                 gridAsset.Rotation = quaternion.identity;
//                 gridAsset.Color = request.DebugColor;
//
//                 // Calculate the number of cells based on MaxCellCount
//                 int widthInCells = math.min(request.WidthInCells, request.MaxCellCount);
//                 int heightInCells = math.min(request.HeightInCells, request.MaxCellCount);
//                 gridAsset.Bounds. = widthInCells;
//                 gridAsset.Height = heightInCells;
//
//                 // Calculate new cell size to ensure the entire terrain is covered
//                 float cellSizeX = request.TerrainSize.x / widthInCells;
//                 float cellSizeZ = request.TerrainSize.z / heightInCells;
//
//                 // Allocate BlobArray for the cells based on the adjusted number of cells
//                 var blobCells = builder.Allocate(ref gridAsset.Cells, widthInCells * heightInCells);
//
//                 for (int row = 0; row < heightInCells; row++)
//                 {
//                     for (int col = 0; col < widthInCells; col++)
//                     {
//                         int index = row * widthInCells + col;
//
//                         // Calculate height from the heightmap blob in request
//                         float heightRatioX = (float)col / (widthInCells - 1);
//                         float heightRatioZ = (float)row / (heightInCells - 1);
//                         int heightMapX = (int)(heightRatioX * (request.HeightmapResolution - 1));
//                         int heightMapZ = (int)(heightRatioZ * (request.HeightmapResolution - 1));
//
//                         // Get the height value from the heightmap
//                         float terrainHeight =
//                             request.HeightmapBlob.Value.Heights[heightMapZ * request.HeightmapResolution + heightMapX];
//
//                         // Calculate the world position of the cell
//                         int2 cellPosition = new int2(col, row);
//                         float3 worldPosition = new float3(
//                             col * cellSizeX,
//                             terrainHeight * request.TerrainSize.y,
//                             row * cellSizeZ
//                         );
//
//                         // Set the cell data
//                         blobCells[index].Position = cellPosition;
//                         blobCells[index].WorldPosition = worldPosition;
//                         blobCells[index].Type = 0; // Default type
//
//                         // Set neighbors (up, down, left, right) with proper handling for edges
//                         var blobNeighbors = builder.Allocate(ref blobCells[index].Neighbors, 4);
//                         blobNeighbors[0] = row > 0 ? (row - 1) * widthInCells + col : -1; // Top neighbor
//                         blobNeighbors[1] =
//                             row < heightInCells - 1 ? (row + 1) * widthInCells + col : -1; // Bottom neighbor
//                         blobNeighbors[2] = col > 0 ? row * widthInCells + (col - 1) : -1; // Left neighbor
//                         blobNeighbors[3] =
//                             col < widthInCells - 1 ? row * widthInCells + (col + 1) : -1; // Right neighbor
//                     }
//                 }
//
//                 GridBlob[0] = builder.CreateBlobAssetReference<GridAssetBlob>(Allocator.Persistent);
//                 builder.Dispose();
//             }
//         }
//     }
// }