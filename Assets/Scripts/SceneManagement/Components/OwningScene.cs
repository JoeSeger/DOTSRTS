using Unity.Entities;
using Hash128 = Unity.Entities.Hash128;

namespace DOTSRTS.SceneManagement.Components
{
    public struct OwningScene : IComponentData
    {
        public Hash128 Guid;
    }
}