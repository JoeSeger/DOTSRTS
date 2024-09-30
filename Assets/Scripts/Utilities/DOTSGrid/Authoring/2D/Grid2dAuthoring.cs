using DOTSRTS.Utilities.DOTSGrid.Components._2D;
using DOTSRTS.Utilities.DOTSGrid.Data;
using DOTSRTS.Utilities.Mono;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTSGrid.Authoring._2D
{
    public class Grid2dAuthoring : MonoBehaviour
    {
        [SerializeField] private int rows = 10;
        [SerializeField] private int columns = 10;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Color debugColor = Color.green;
        [SerializeField] private Axis rotationAxis = Axis.Y;

        // Optionally show gizmos for debugging
        private void OnDrawGizmos()
        {
            float3 extents = CalculateExtents(rows, columns, cellSize);
            float3 center = transform.position;

            Gizmos.color = debugColor;
            Gizmos.DrawWireCube(center, extents * 2f); // Drawing the bounds
        }

        // Function to calculate extents based on rows, columns, and cell size
        private float3 CalculateExtents(int calRows, int calColumns, float calCellSize)
        {
            float halfWidth = (calColumns * calCellSize) / 2f;
            float halfHeight = (calRows * calCellSize) / 2f;
            return new float3(halfWidth, 0, halfHeight);
        }

        // Baker class for converting MonoBehaviour to ECS data
        public class Grid2dBaker : Baker<Grid2dAuthoring>
        {
            public override void Bake(Grid2dAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<Grid2d>(entity);
                AddComponent<GridComponent>(entity);

                // Generate the request for grid creation
                AddComponent(entity, new Generate2DGridRequest
                {
                    Rows = authoring.rows,
                    Columns = authoring.columns,
                    CellSize = authoring.cellSize,
                    DebugColor = authoring.debugColor.ToFloat4(),
                    RotationAxis = authoring.rotationAxis
                });
            }
        }
    }
}