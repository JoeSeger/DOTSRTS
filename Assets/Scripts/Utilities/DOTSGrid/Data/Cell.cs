

using System;
using System.Collections.Generic;
using DOTSRTS.Utilities.PrefabsManagement.Authoring;
using Sirenix.OdinInspector;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace DOTSRTS.Utilities.DOTSGrid.Data
{
    
    
    public struct Cell
    {
        public int2 GridPosition;
        public float3 WorldPosition;
        public BlobArray<float3> Corners;  // Store the four corners of the cell
        public BlobArray<Cell> Neighbors;
        public CellType Type;
        public BoundsFloat3 Bounds;
        
        public float3 GetCorner(Corner corner) => Corners[(int)corner];
    }

    [Serializable]
    public class SerializableCell
    {
        public int2 gridPosition;
        public float3 worldPosition;
        
        [HideInInspector] 
        public List<SerializableCell> neighbors;
        public List<float3> corners;
        public CellType type;
        [ShowInInspector]public BoundsFloat3 Bounds => GetBounds();
        public float3 GetCorner(Corner corner) => corners[(int)corner];
        [ShowInInspector]public bool SpawnPrefab => type.HasFlag(CellType.PrefabSpawnAble);
        
       [ShowIf("SpawnPrefab")]public BakingPrefabData prefabData;
        private BoundsFloat3 GetBounds()
        {
            // Ensure there are enough corners (you should have 4 corners at least)
            if (corners.Count < 4)
            {
                return new BoundsFloat3(float3.zero, float3.zero);
            }

            // Get the top-left and bottom-right corners
            var topLeft = GetCorner(Corner.TopLeft);
            var bottomRight = GetCorner(Corner.BottomRight);

            // Calculate the bounds using the corners' positions
            var size = bottomRight - topLeft;
            var center = topLeft + size * 0.5f; // Center is midway between corners

            return new BoundsFloat3(center, size);
        }



 
        
        
    }
    
}