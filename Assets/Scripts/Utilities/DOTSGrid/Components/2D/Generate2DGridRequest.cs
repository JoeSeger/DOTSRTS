using DOTSRTS.Utilities.DOTSGrid.Data;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTSRTS.Utilities.DOTSGrid.Components._2D
{
    public struct Generate2DGridRequest : IComponentData
    { // Grid bounds that define the min, max, and center
        public float4 DebugColor;        // Debug color to visualize the grid in the editor
        public int Rows;                 // Number of rows in the grid
        public int Columns;              // Number of columns in the grid
        public float CellSize;           // Size of each grid cell
        public Axis RotationAxis;        // The axis to rotate the grid around
    }
}