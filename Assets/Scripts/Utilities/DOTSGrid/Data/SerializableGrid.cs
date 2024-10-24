using System.Collections.Generic;
using System.Linq;
using DOTSRTS.Utilities.DOTSGrid.Tools;
using RotaryHeart.Lib.SerializableDictionaryPro;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;


namespace DOTSRTS.Utilities.DOTSGrid.Data
{
    [System.Serializable]
    public class SerializableGrid
    {
        public int ID;
        public int Layer => layer;
        public int mouseOver;
        public float CellSize => cellSize;
        public int2 Scale => scale;
        public int TotalCells => scale.x * scale.y;
        public Axis RotationAxis => rotationAxis;
        
        [SerializeField] private int layer;
        [SerializeField] private float cellSize = 1;

        [SerializeField]
        private int2 scale = new(10, 10); // where scale.x = columns and scale.y = rows

        [SerializeField]
        private Axis rotationAxis;
        

        public float3 origin;
        public float2 HalfScale => GetHalfScale();
        public float2 Dimension => GetDimension();
        public quaternion Rotation => GetRotation();

        public BoundsFloat3 Bounds => GetBounds();
        public void UpdateCell(SerializableCell updatedCell)
        {
            // Find the index of the cell with the same gridPosition
            int index = cells.FindIndex(cell => cell.gridPosition.Equals(updatedCell.gridPosition));
    
            // If the cell is found, update it
            if (index >= 0)
            {
                cells[index] = updatedCell;
            }
            else
            {
                // Optionally, you can add the cell if it doesn't exist
                cells.Add(updatedCell);
            }
        }
        

        private BoundsFloat3 GetBounds()
        {
            // Ensure there are enough corners (you should have 4 corners at least)
            if (corners.Count < 4)
            {
                return new BoundsFloat3(float3.zero, float3.zero);
            }

            // Get the top-left and bottom-right corners
            var topLeft = GetCorner(Corner.TopLeft).GetCorner(Corner.TopLeft);
            var bottomRight = GetCorner(Corner.BottomRight).GetCorner(Corner.BottomRight);

            // Calculate the bounds using the corners' positions
            var size = bottomRight - topLeft;
            var center = topLeft + size * 0.5f; // Center is midway between corners

            return new BoundsFloat3(center, size);
        }

        public List<SerializableCell> cells = new();
        [HideInInspector] public List<SerializableCell> corners = new();

        private quaternion GetRotation() => GridTools.ApplyAxisRotation(rotationAxis);

        [ShowInInspector]
        public bool SpawnsPrefabs => cells.Any(cell => cell.SpawnPrefab);
       

        private float2 GetDimension() => new(scale.x * cellSize, scale.y * cellSize);
        private float2 GetHalfScale() => new(Dimension.x / 2, Dimension.y / 2);

        public SerializableCell GetCorner(Corner corner) => corners?[(int)corner];

        public SerializableDictionary<CellType, Color> debugCellTypeColorData = CellTypeDebugTools.GetDebugCellTypeColorData();
        public CellType ViewingCellTypes;
  
        
        public SerializableCell GetCell(int2 gridPosition)
        {
            return cells.FirstOrDefault(cell => cell.gridPosition.Equals(gridPosition));
        }
       

        public SerializableCell GetCell(ref Cell cell)
        {
            return cells.FirstOrDefault(cell => cell.gridPosition.Equals(cell.gridPosition));
        }
      
        [Button]
        public void ClearTypes()
        {
            foreach (var cell in cells)
            {
                cell.type = default;
            }   
        }
        
        public Color gridColor = Color.green;
        
    }
}