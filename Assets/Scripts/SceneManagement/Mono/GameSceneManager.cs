using DOTSRTS.SceneManagement.Components;
using DOTSRTS.Utilities.Mono.Singletons;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Entities;

namespace DOTSRTS.SceneManagement.Mono
{
    public class GameSceneManager : SerializedMonoSingletonPersistent<GameSceneManager>
    {
        [Button]
        public  void LoadScene(SceneLoadRequest sceneLoadRequest,World world = null,EntityManager entityManager = default)
        {
            world ??= World.DefaultGameObjectInjectionWorld;

            if (entityManager == default)
            {
                entityManager = world.EntityManager;
            }
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var loadSceneEntity = ecb.CreateEntity();
            ecb.AddComponent(loadSceneEntity, sceneLoadRequest);
            ecb.Playback(entityManager);
            ecb.Dispose();
        }
    }
}