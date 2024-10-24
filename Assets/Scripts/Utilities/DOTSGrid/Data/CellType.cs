using System;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionaryPro;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTSGrid.Data
{
    [Flags]
    public enum CellType
    {
        WalkAble = 1 << 0, // 1: The cell can be walked on
        NoneWalkAble = 1 << 1, // 2: The cell cannot be walked on
        PrefabSpawnAble = 1 << 2, // 4: The cell can spawn prefabs
        Obstacle = 1 << 3, // 8: The cell contains an obstacle
        Water = 1 << 4, // 16: The cell contains water
        Hazardous = 1 << 5, // 32: The cell contains hazards like traps or lava
        ItemSpawn = 1 << 6, // 64: The cell can spawn items or power-ups
        Occupied = 1 << 7, // 128: The cell is occupied by another entity
        Goal = 1 << 8, // 256: The cell represents a goal or destination
        Interactable = 1 << 9, // 512: The cell can trigger events when interacted with
        Checkpoint = 1 << 10, // 1024: The cell represents a checkpoint
        Jumpable = 1 << 11, // 2048: The cell allows jumping mechanics
        Cover = 1 << 12, // 4096: The cell provides cover in tactical games
        Sky = 1 << 13, // 8192: The cell represents sky (air space)
        Ground = 1 << 14, // 16384: The cell represents ground (land terrain)
        Amount = 15 // Special case to indicate error
    }

    public static class CellTypeDebugTools
    {

        public static SerializableDictionary<CellType, bool> GetDefaultDisplayDebugBools(CellType cellType)
        {
            var returningDefault = new Dictionary<CellType, bool>((int)CellType.Amount);

            // Loop through each CellType value and check if it is set in the provided cellType
            foreach (CellType type in Enum.GetValues(typeof(CellType)))
            {
                if (type is CellType.Amount) continue; // Skip special values

                // Check if the cellType contains this flag
                bool isSet = (cellType & type) == type;

                // Add the result to the dictionary
                returningDefault.Add(type, isSet);
            }
        
            return returningDefault.ToSerializableDictionary();
        }
    
        public static SerializableDictionary<T1, T2> ToSerializableDictionary<T1,T2>(this Dictionary<T1, T2> dictionary)
        {
            // Create a new SerializableDictionary instance
            var serializableDictionary = new SerializableDictionary<T1, T2>();

            // Loop through each entry in the provided dictionary
            foreach (var kvp in dictionary)
            {
                // Add the key-value pair to the SerializableDictionary
                serializableDictionary.Add(kvp.Key, kvp.Value);
            }

            // Return the populated SerializableDictionary
            return serializableDictionary;
        }
        public const CellType InvalidCellType = CellType.Amount ;

        public static SerializableDictionary<CellType, Color> GetDebugCellTypeColorData()
        {
            Dictionary<CellType, Color> returningData = new Dictionary<CellType, Color>();

            // Loop through all possible CellType values (based on the flags in the enum)
            foreach (CellType cellType in Enum.GetValues(typeof(CellType)))
            {
                // Assign specific colors to each CellType
                switch (cellType)
                {
                    case CellType.WalkAble:
                        returningData.Add(cellType, Color.green); // Walkable areas are green
                        break;
                    case CellType.NoneWalkAble:
                        returningData.Add(cellType, Color.red); // Non-walkable areas are red
                        break;
                    case CellType.PrefabSpawnAble:
                        returningData.Add(cellType, Color.yellow); // Spawnable areas are yellow
                        break;
                    case CellType.Obstacle:
                        returningData.Add(cellType, Color.gray); // Obstacles are gray
                        break;
                    case CellType.Water:
                        returningData.Add(cellType, Color.blue); // Water is blue
                        break;
                    case CellType.Hazardous:
                        returningData.Add(cellType, new Color(1f, 0.5f, 0f)); // Hazardous areas are orange
                        break;
                    case CellType.ItemSpawn:
                        returningData.Add(cellType, new Color(0.5f, 1f, 0.5f)); // Item spawn areas are light green
                        break;
                    case CellType.Occupied:
                        returningData.Add(cellType, new Color(0.5f, 0f, 0.5f)); // Occupied areas are purple
                        break;
                    case CellType.Goal:
                        returningData.Add(cellType, Color.magenta); // Goals are magenta
                        break;
                    case CellType.Interactable:
                        returningData.Add(cellType, Color.cyan); // Interactable areas are cyan
                        break;
                    case CellType.Checkpoint:
                        returningData.Add(cellType, Color.yellow); // Checkpoints are yellow
                        break;
                    case CellType.Jumpable:
                        returningData.Add(cellType, Color.white); // Jumpable areas are white
                        break;
                    case CellType.Cover:
                        returningData.Add(cellType, new Color(0.3f, 0.3f, 0.3f)); // Cover areas are dark gray
                        break;
                    case CellType.Sky:
                        returningData.Add(cellType, Color.cyan); // Sky is cyan
                        break;
                    case CellType.Ground:
                        returningData.Add(cellType, new Color(101f / 255f, 67f / 255f, 33f / 255f)); // Ground is dark brown
                        break;
                    case CellType.Amount:
                        returningData.Add(cellType, Color.red); // Error case is red
                        break;
                    default:
                        returningData.Add(cellType, Color.clear); // Default is clear
                        break;
                }
            }

            return returningData.ToSerializableDictionary();
        }
    }
}