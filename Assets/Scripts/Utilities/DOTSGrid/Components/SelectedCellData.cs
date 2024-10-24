using DOTSRTS.Utilities.DOTSGrid.Data;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTSRTS.Utilities.DOTSGrid.Components
{
    public struct SelectedCellData : IComponentData
    {
        public int GridID;
        public int GridInstanceID;
        public int2 GridPosition;
        public BoundsFloat3 Bounds;
        public float3 WorldPosition;
        public int Type;
        public bool CellIsSelected;
    }
}