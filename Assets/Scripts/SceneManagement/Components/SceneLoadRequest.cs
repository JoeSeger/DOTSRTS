using Unity.Entities;
using UnityEngine.SceneManagement;

namespace DOTSRTS.SceneManagement.Components
{
    public struct SceneLoadRequest : IComponentData
    {
        public int BuildIndex;
        public LoadSceneParameters LoadSceneParameters;
    }
}