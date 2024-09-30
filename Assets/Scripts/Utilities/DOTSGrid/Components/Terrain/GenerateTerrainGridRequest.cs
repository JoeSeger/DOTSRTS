using Unity.Entities;
using Unity.Mathematics;

namespace DOTSRTS.Utilities.DOTSGrid.Components.Terrain
{
    public struct GenerateTerrainGridRequest : IComponentData
    {
        public float3 TerrainSize; // The size of the terrain (width, height, depth)
        public int HeightmapResolution; // The resolution of the heightmap
        public int WidthInCells; // Number of grid cells in width
        public int HeightInCells; // Number of grid cells in height
        public float4 DebugColor;
        public BlobAssetReference<HeightmapBlob> HeightmapBlob; // Blob reference for terrain heights
        public int MaxCellCount;
    }
    // Define a blob to store the heightmap
    public struct HeightmapBlob
    {
        public BlobArray<float> Heights; // Blob array for heights
    }
}