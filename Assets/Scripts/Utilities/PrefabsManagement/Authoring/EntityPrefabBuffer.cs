using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;

namespace DOTSRTS.Utilities.PrefabsManagement.Authoring
{
    public struct EntityPrefabBuffer : IBufferElementData
    {
        public FixedString128Bytes Name;
        public Hash128 Guid;
        public EntityPrefabReference EntityPrefabReference;
    }
}