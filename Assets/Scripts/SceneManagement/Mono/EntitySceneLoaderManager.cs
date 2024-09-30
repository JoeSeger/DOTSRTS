using DOTSRTS.SceneManagement.Data;
using DOTSRTS.Utilities.Mono.Singletons;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DOTSRTS.SceneManagement.Mono
{
    public partial class EntitySceneLoaderSystem : SystemBase
    {
        private GameSceneManagerEventAggregator _eventAggregator;
        private Entity _sceneLoaderManagerEntity;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            // Find the GameSceneManagerEventAggregator instance in the scene
            _eventAggregator = GameSceneManagerEventAggregator.Instance;

            if (_eventAggregator != null)
            {
                // Subscribe to the events
                _eventAggregator.Subscribe<SceneLoadingStartedEvent>(OnSceneLoadingStarted);
                _eventAggregator.Subscribe<SceneLoadingProgressEvent>(OnSceneLoadingProgress);
                _eventAggregator.Subscribe<SceneLoadingCompletedEvent>(OnSceneLoadingCompleted);
                _eventAggregator.Subscribe<SceneLoadingCanceledEvent>(OnSceneLoadingCanceled);
            }
            else
            {
                Debug.LogError("Could not find GameSceneManagerEventAggregator");
            }

            // Create an entity to attach the scene tags
            
            _sceneLoaderManagerEntity = EntityManager.CreateEntity();
            EntityManager.SetName(_sceneLoaderManagerEntity,"SceneManager");
        }

        
        protected override void OnUpdate()
        {
            // The OnUpdate method is where you can implement any per-frame logic if necessary.
            // In this case, most of the logic is event-driven, so OnUpdate might not need much additional code.
        }

        private void OnSceneLoadingStarted(SceneLoadingStartedEvent eventArgs)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            RemoveAllSceneTags(ecb);

            // Add the SceneLoadingStartedTag
            ecb.AddComponent(_sceneLoaderManagerEntity, new SceneLoadingStartedTag
            {
                BuildIndex = eventArgs.Value.buildIndex
            });

          
            ecb.Playback(EntityManager);
            ecb.Dispose();
            
        }

        private void OnSceneLoadingProgress(SceneLoadingProgressEvent eventArgs)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            RemoveAllSceneTags(ecb);

            // Add the SceneLoadingProgressTag
            ecb.AddComponent(_sceneLoaderManagerEntity, new SceneLoadingProgressTag
            {
                BuildIndex = eventArgs.Value.buildIndex,
                Progress = eventArgs.Progress
            });
            
            ecb.Playback(EntityManager);
            ecb.Dispose();
            
        }

        private void OnSceneLoadingCompleted(SceneLoadingCompletedEvent eventArgs)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            RemoveAllSceneTags(ecb);

            // Add the SceneLoadingCompletedTag
            ecb.AddComponent(_sceneLoaderManagerEntity, new SceneLoadingCompletedTag
            {
                BuildIndex = eventArgs.Value.buildIndex
            });
            
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        private void OnSceneLoadingCanceled(SceneLoadingCanceledEvent eventArgs)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            RemoveAllSceneTags(ecb);

            // Add the SceneLoadingCanceledTag
            ecb.AddComponent(_sceneLoaderManagerEntity, new SceneLoadingCanceledTag
            {
                BuildIndex = eventArgs.Value.buildIndex
            });
            ecb.Playback(EntityManager);
            ecb.Dispose();
            
        }

        private void RemoveAllSceneTags(EntityCommandBuffer entityCommandBuffer)
        {
            entityCommandBuffer.RemoveComponent<SceneLoadingStartedTag>(_sceneLoaderManagerEntity);
            entityCommandBuffer.RemoveComponent<SceneLoadingProgressTag>(_sceneLoaderManagerEntity);
            entityCommandBuffer.RemoveComponent<SceneLoadingCompletedTag>(_sceneLoaderManagerEntity);
            entityCommandBuffer.RemoveComponent<SceneLoadingCanceledTag>(_sceneLoaderManagerEntity);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_eventAggregator == null) return;
            // Unsubscribe from the events when the system is destroyed
            _eventAggregator.Unsubscribe<SceneLoadingStartedEvent>(OnSceneLoadingStarted);
            _eventAggregator.Unsubscribe<SceneLoadingProgressEvent>(OnSceneLoadingProgress);
            _eventAggregator.Unsubscribe<SceneLoadingCompletedEvent>(OnSceneLoadingCompleted);
            _eventAggregator.Unsubscribe<SceneLoadingCanceledEvent>(OnSceneLoadingCanceled);
        }
    }
}
