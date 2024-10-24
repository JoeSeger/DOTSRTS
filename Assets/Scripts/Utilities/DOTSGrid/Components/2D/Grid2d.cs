using DOTSRTS.Utilities.DOTSGrid.Data;
using Unity.Entities;

namespace DOTSRTS.Utilities.DOTSGrid.Components._2D
{
    public struct Grid2d : IComponentData
    {
    }

#if UNITY_EDITOR
    public class SerializableGridData : IComponentData
    {
        public SerializableGrid Value;
    }
#endif
   
    
}