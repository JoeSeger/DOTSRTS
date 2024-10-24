using Unity.Entities;

namespace DOTSRTS.Utilities.DOTSGrid.Tools
{
    public static class DOTSTools
    {
        public static EntityManager GetDefaultEntityManager()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            return world?.EntityManager ?? default;
        }

    }
}