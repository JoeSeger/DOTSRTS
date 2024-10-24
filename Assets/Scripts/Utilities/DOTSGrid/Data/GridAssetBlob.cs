using Unity.Entities;
using Unity.Mathematics;


namespace DOTSRTS.Utilities.DOTSGrid.Data
{
    public enum Corner
    {
        Invalid = -1,

        TopLeft = 0, // Top-left
        TopRight, // Top-right
        BottomLeft, // Bottom-left
        BottomRight, // Bottom-right

        Amount = 4
    }

    public struct GridAssetBlob
    {
        public BlobArray<Cell> Corners;
        public BlobArray<Cell> Cells;
    }
}