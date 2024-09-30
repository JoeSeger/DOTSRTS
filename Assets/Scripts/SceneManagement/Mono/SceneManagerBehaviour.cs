
using DOTSRTS.SceneManagement.Data;
using UnityEngine;

namespace DOTSRTS.SceneManagement.Mono
{
    public sealed class SceneManagerBehaviour: MonoBehaviour
    {
        [SerializeField] private GameSceneManagerEventAggregator eventAggregator;


        private void OnEnable()
        {
            eventAggregator.Subscribe<SceneLoadingStartedEvent>(OnSceneLoading);
            eventAggregator.Subscribe<SceneLoadingProgressEvent>(OnSceneLoadingProgressEvent);
            eventAggregator.Subscribe<SceneLoadingCompletedEvent>(OnSceneLoadingCompletedEvent);
            eventAggregator.Subscribe<SceneLoadingCanceledEvent>(OnSceneLoadingCanceledEvent);
        }

        private void OnSceneLoading(SceneLoadingStartedEvent eventArgs)
        {
           
        }

        private void OnSceneLoadingProgressEvent(SceneLoadingProgressEvent eventArgs)
        {
        }

        private void OnSceneLoadingCompletedEvent(SceneLoadingCompletedEvent eventArgs)
        {
            
        }

        private void OnSceneLoadingCanceledEvent(SceneLoadingCanceledEvent eventArgs)
        {
        }
        private void OnDisable()
        {
            eventAggregator.Unsubscribe<SceneLoadingStartedEvent>(OnSceneLoading);
            eventAggregator.Unsubscribe<SceneLoadingProgressEvent>(OnSceneLoadingProgressEvent);
            eventAggregator.Unsubscribe<SceneLoadingCompletedEvent>(OnSceneLoadingCompletedEvent);
            eventAggregator.Unsubscribe<SceneLoadingCanceledEvent>(OnSceneLoadingCanceledEvent);
        }
    }
}