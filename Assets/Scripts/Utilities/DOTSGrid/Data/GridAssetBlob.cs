using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;


namespace DOTSRTS.Utilities.DOTSGrid.Data
{
    public enum Corner
    {
        Invalid = -1,
        
        TopLeft,  // Top-left
        TopRight,  // Top-right
        BottomLeft, // Bottom-left
        BottomRight, // Bottom-right
        
        Amount = 4
    }
    public struct GridAssetBlob
    {
        public BlobArray<Cell> Corners;
        public BlobArray<Cell> Cells;
        public float4 Color;
        public float CellSize;
        public Float3Bounds Bounds;
        public Axis RotationAxis; // Store the axis instead of quaternion

        public Cell GetCorner(Corner corner) => Corners[(int)corner];
        
    }
    
    
}