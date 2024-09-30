using System.Threading;
using DOTSRTS.SceneManagement.Components;
using DOTSRTS.SceneManagement.Mono;
using Unity.Collections;
using Unity.Entities;


namespace DOTSRTS.SceneManagement.Systems
{
    public partial class SceneManagerSystem : SystemBase
    {

        private CancellationToken _cancellationToken;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<SceneLoadRequest>();
            _cancellationToken = new CancellationToken();
        }


        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity, ref SceneLoadRequest sceneLoadRequest) => {
                SceneManagerTools.LoadScene(sceneLoadRequest,_cancellationToken);
                ecb.DestroyEntity(entity);
            }).Run();
               
            ecb.Playback(EntityManager);
        }

        protected override void OnDestroy()
        {
            if (_cancellationToken.CanBeCanceled)
            {
                _cancellationToken.ThrowIfCancellationRequested();
            }
      
            base.OnDestroy();
        }
    }
}