using DOTSRTS.Utilities.DOTSGrid.Components.Terrain;
using DOTSRTS.Utilities.DOTSGrid.Data;
using DOTSRTS.Utilities.Mono;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTSGrid.Authoring.Terrain
{
    public class TerrainGridAuthoring : MonoBehaviour
    {
        [SerializeField] private int iD;
        [SerializeField] private int layer;
        [SerializeField] private TerrainData terrainData;
        [SerializeField] private Color debugColor;
        [SerializeField] private int maxCellCount;

        public class TerrainGridBaker : Baker<TerrainGridAuthoring>
        {
            public override void Bake(TerrainGridAuthoring authoring)
            {
                // Get the terrain data
                var terrainData = authoring.terrainData;

                // Create an entity for this grid
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                // Calculate the number of cells in width and height, limited by the max cell count
                var terrainSize = terrainData.size;
                var maxCells = math.min(authoring.maxCellCount, terrainData.heightmapResolution);

                var widthInCells = math.clamp(terrainData.heightmapResolution, 1, maxCells);
                var heightInCells = math.clamp(terrainData.heightmapResolution, 1, maxCells);

                // Create the heightmap blob asset
                var heightmapBlob = CreateHeightmapBlob(terrainData);

                // Create the GenerateTerrainGridRequest and add it as a component
                AddComponent(entity, new GenerateTerrainGridRequest
                {
                    TerrainSize = terrainSize,
                    HeightmapResolution = terrainData.heightmapResolution,
                    WidthInCells = widthInCells, // Calculated width in cells
                    HeightInCells = heightInCells, // Calculated height in cells
                    DebugColor = authoring.debugColor.ToFloat4(),
                    HeightmapBlob = heightmapBlob,
                    MaxCellCount = authoring.maxCellCount
                });

                // Add the required components
                AddComponent(entity, new GridComponent
                    {
                        ID = authoring.iD,
                        Layer = authoring.layer,
                        GridAsset = new BlobAssetReference<GridAssetBlob>()
                    }
                );
                AddComponent<TerrainGrid>(entity);
            }

            private static BlobAssetReference<HeightmapBlob> CreateHeightmapBlob(TerrainData terrainData)
            {
                // Get the heightmap resolution and the height values
                var heightmapResolution = terrainData.heightmapResolution;
                var heights = terrainData.GetHeights(0, 0, heightmapResolution, heightmapResolution);

                // Flatten the 2D height array into a 1D array and create the blob asset
                var builder = new BlobBuilder(Allocator.Temp);
                ref var root = ref builder.ConstructRoot<HeightmapBlob>();
                var blobArray = builder.Allocate(ref root.Heights, heightmapResolution * heightmapResolution);

                for (var y = 0; y < heightmapResolution; y++)
                {
                    for (var x = 0; x < heightmapResolution; x++)
                    {
                        blobArray[y * heightmapResolution + x] = heights[y, x];
                    }
                }

                // Create the blob asset reference and return it
                var heightmapBlob = builder.CreateBlobAssetReference<HeightmapBlob>(Allocator.Persistent);
                builder.Dispose();

                return heightmapBlob;
            }
        }
    }
}