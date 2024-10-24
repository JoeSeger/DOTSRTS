using Unity.Entities;
using Unity.Mathematics;


namespace DOTSRTS.Utilities.DOTSGrid.Data
{
    public struct GridComponent : IComponentData
    {
        public int ID;
        public int Layer;
        public bool MouseOver;
        public float CellSize;
        public int2 Scale;
        public float2 HalfScale;
        public Axis RotationAxis;
        public quaternion Rotation;
        public BlobAssetReference<GridAssetBlob> GridAsset;
        public float3 Origin;
        public BoundsFloat3 Bounds;
        public int InstanceID;
    }
    
    public struct DebugGridData : IComponentData
    {
        public CellType ViewingCellTypes;
        public float4 DebugColor;
    }

    [InternalBufferCapacity((int)CellType.Amount)]
    public struct DebugGridCellTypeData : IBufferElementData
    {
        public CellType CellType;
        public float4 Color;
    }
}