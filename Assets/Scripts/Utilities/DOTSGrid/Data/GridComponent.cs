using Unity.Entities;
using Unity.Mathematics;

namespace DOTSRTS.Utilities.DOTSGrid.Data
{
    public struct GridComponent : IComponentData
    {
        public int ID;
        public int Layer;
        public bool MouseOver;
        public float3 CenterPosition;
        public BlobAssetReference<GridAssetBlob> GridAsset;
    }
}