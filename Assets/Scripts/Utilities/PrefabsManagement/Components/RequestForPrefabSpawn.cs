using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTSRTS.Utilities.PrefabsManagement.Components
{
    public struct PrefabSpawnRequest: IComponentData
    {
        public Hash128 guid;
        public float3 position;
        public quaternion rotation;
        public float Scale;
    }
}