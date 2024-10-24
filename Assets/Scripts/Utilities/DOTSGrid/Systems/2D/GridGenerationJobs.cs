using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace DOTSRTS.Utilities.DOTSGrid.Systems._2D
{
    public static class GridGenerationJobs
    {
        [BurstCompile]
        public struct GenerateGridJob : IJobParallelFor
        {
            public int2 Scale;
            public float CellSize;
            public float3 Origin;
            public float3 CenterPosition;
            public quaternion Rotation;

            public NativeArray<float3> WorldPositions;
            public NativeArray<int2> GridPositions;
            

            public void Execute(int index)
            {
                int col = index % Scale.x;
                int row = index / Scale.x;

                float3 localPosition = new float3(col * CellSize, 0, row * CellSize);
                var worldPosition = Origin + localPosition;
                float3 offset = worldPosition - CenterPosition;

                WorldPositions[index] = math.mul(Rotation, offset) + CenterPosition;
                GridPositions[index] = new int2(col, row);
            }
        }

        [BurstCompile]
        public struct SetCellCornersJob : IJobParallelFor
        {
            public float CellSize;
            public quaternion Rotation;
            [ReadOnly] public NativeArray<float3> WorldPositions;
            public NativeArray<float3x4> Corners;

            public void Execute(int index)
            {
                float halfCellSize = CellSize / 2f;
                float3 centerPosition = WorldPositions[index];

                float3 topLeft = new float3(-halfCellSize, 0, -halfCellSize);
                float3 topRight = new float3(halfCellSize, 0, -halfCellSize);
                float3 bottomLeft = new float3(-halfCellSize, 0, halfCellSize);
                float3 bottomRight = new float3(halfCellSize, 0, halfCellSize);

                Corners[index] = new float3x4
                {
                    c0 = math.mul(Rotation, topLeft) + centerPosition,
                    c1 = math.mul(Rotation, topRight) + centerPosition,
                    c2 = math.mul(Rotation, bottomLeft) + centerPosition,
                    c3 = math.mul(Rotation, bottomRight) + centerPosition
                };
            }
        }

        [BurstCompile]
        public struct SetGridCornersJob : IJob
        {
            public int Rows;
            public int Columns;
            public NativeArray<int> CornerIndices;

            public void Execute()
            {
                CornerIndices[0] = 0; // Top-left
                CornerIndices[1] = Columns - 1; // Top-right
                CornerIndices[2] = (Rows - 1) * Columns; // Bottom-left
                CornerIndices[3] = (Rows * Columns) - 1; // Bottom-right
            }
        }
    }
}