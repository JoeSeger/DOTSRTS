using Unity.Entities;
using UnityEngine.SceneManagement;

namespace DOTSRTS.SceneManagement.Mono
{
    public struct SceneLoadingProgressEvent
    {
        public Scene Value { get; set; }
        public float Progress { get; set; }
    }

    public struct SceneLoadingStartedEvent
    {
        public Scene Value { get; set; }
    }

    public struct SceneLoadingCompletedEvent
    {
        public Scene Value { get; set; }
    }

    public struct SceneLoadingCanceledEvent
    {
        public Scene Value { get; set; }
    }
    
    public struct SceneLoadingProgressTag: IComponentData
    {
        public int BuildIndex;
        public float Progress;
    }

    public struct SceneLoadingStartedTag : IComponentData
    {
        public int BuildIndex;
    }

    public struct SceneLoadingCompletedTag : IComponentData
    {
        public int BuildIndex;
    }

    public struct SceneLoadingCanceledTag : IComponentData
    {
        public int BuildIndex;
    }
}