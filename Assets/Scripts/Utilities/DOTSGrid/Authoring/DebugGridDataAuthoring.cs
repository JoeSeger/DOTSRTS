using System;
using DOTSRTS.Utilities.DOTSGrid.Authoring._2D;
using DOTSRTS.Utilities.DOTSGrid.Data;
using DOTSRTS.Utilities.DOTSGrid.Systems._2D;
using DOTSRTS.Utilities.Mono;
using Unity.Entities;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTSGrid.Authoring
{
    public class DebugGridDataAuthoring : MonoBehaviour
    {
        public class DebugGridDataBaker : Baker<DebugGridDataAuthoring>
        {
            public override void Bake(DebugGridDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
         

                var gridComponent = authoring.GetComponent<Grid2dAuthoring>();
                DependsOn(gridComponent);

         
                var gridData = GridData.Instance;
                if(gridData == null) return;
                DependsOn(gridComponent.GridData);
                
                var gridID = gridComponent.ID;
                var grid = gridData.SerializableGrids[gridID];
                
                if(grid == null) return;
                
                AddComponent(entity,new DebugGridData
                {
                    ViewingCellTypes = grid.ViewingCellTypes,
                    DebugColor = grid.gridColor.ToFloat4()
                });
                
                DynamicBuffer<DebugGridCellTypeData> myBuff = AddBuffer<DebugGridCellTypeData>(entity);

                foreach (CellType cellType in Enum.GetValues(typeof(CellType)))
                {
                    // Ensure grid.debugCellTypeColorData contains the CellType
                    if (grid.debugCellTypeColorData.TryGetValue(cellType, out Color color))
                    {
                        myBuff.Add(new DebugGridCellTypeData
                        {
                            CellType = cellType,
                            Color = color.ToFloat4() // Assuming ToFloat4() is a correct extension method
                        });
                    }
                }
              
                
                
            }
        }
    }
}