using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTSGrid.Data
{
    public struct Cell
    {
        public int2 GridPosition;
        public float3 WorldPosition;
        public BlobArray<float3> Corners;  // Store the four corners of the cell
        public BlobArray<int> Neighbors;
        public int Type;

        public float3 GetCorner(Corner corner)
        {
            var c = Corners[(int)corner];
            return c;
        }
    }

    public struct CellComponent : IComponentData
    {
        public int2 Position;
        public BlobArray<int> Neighbors;
    }

}