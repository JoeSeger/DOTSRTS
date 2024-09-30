using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DOTSRTS.SceneManagement.Components;
using DOTSRTS.SceneManagement.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DOTSRTS.SceneManagement.Mono
{
    public static class SceneManagerTools
    {
        public static GameSceneManagerEventAggregator GameSceneManagerEventAggregator
        {
            get
            {
                if (_gameSceneManagerEventAggregator == null)
                {
                    _gameSceneManagerEventAggregator = Resources.FindObjectsOfTypeAll<GameSceneManagerEventAggregator>()
                        .FirstOrDefault();
                }

                return _gameSceneManagerEventAggregator;
            }
        }

        private static GameSceneManagerEventAggregator _gameSceneManagerEventAggregator;

        public static Scene GetSceneFromIdentifier(object sceneIdentifier)
        {
            var scene = sceneIdentifier switch
            {
                int buildIndex => SceneManager.GetSceneByBuildIndex(buildIndex),
                string sceneName => SceneManager.GetSceneByName(sceneName),
                _ => default
            };

            if (!scene.IsValid())
            {
                throw new InvalidOperationException("Scene could not be found or is not valid.");
            }

            return scene;
        }

        public static bool SceneIsLoaded(object sceneIdentifier)
        {
            var scene = GetSceneFromIdentifier(sceneIdentifier);
            return scene.isLoaded;
        }

        public static IEnumerable<Scene> GetLoadedScenes()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                yield return SceneManager.GetSceneAt(i);
            }
        }


        public static void LoadScene(SceneLoadRequest sceneLoadRequest, CancellationToken cancellationToken = default)
        {
            LoadSceneAsync(sceneLoadRequest.BuildIndex, sceneLoadRequest.LoadSceneParameters, cancellationToken);
        }

        public static void LoadScene(string sceneIdentifier, LoadSceneParameters loadSceneParameters,
            CancellationToken cancellationToken = default)
        {
            LoadSceneAsync(sceneIdentifier, loadSceneParameters, cancellationToken);
        }

        public static void LoadScene(int sceneIdentifier, LoadSceneParameters loadSceneParameters,
            CancellationToken cancellationToken = default)
        {
            LoadSceneAsync(sceneIdentifier, loadSceneParameters, cancellationToken);
        }

        public static void LoadScene(object sceneIdentifier, LoadSceneMode mode = LoadSceneMode.Single)
        {
            LoadSceneAsync(sceneIdentifier, new LoadSceneParameters(mode));
        }

        public static void LoadScene(object sceneIdentifier,
            LocalPhysicsMode localPhysicsMode = LocalPhysicsMode.Physics3D, LoadSceneMode mode = LoadSceneMode.Single)
        {
            LoadSceneAsync(sceneIdentifier, new LoadSceneParameters(mode, localPhysicsMode));
        }

        public static void LoadScene(object sceneIdentifier, LoadSceneParameters loadSceneParameters,
            CancellationToken cancellationToken = default)
        {
            LoadSceneAsync(sceneIdentifier, loadSceneParameters, cancellationToken);
        }


        private static async Task ProcessLoadSceneAsync(object sceneIdentifier, LoadSceneParameters loadSceneParameters,
            CancellationToken cancellationToken = default)
        {
            // Check if cancellation is already requested
            cancellationToken.ThrowIfCancellationRequested();

            var asyncOperation = sceneIdentifier switch
            {
                int buildIndex => SceneManager.LoadSceneAsync(buildIndex, loadSceneParameters),
                string sceneName => SceneManager.LoadSceneAsync(sceneName, loadSceneParameters),
                _ => throw new ArgumentException("Invalid scene identifier type. Expected int or string.")
            };

            if (asyncOperation == null)
            {
                throw new InvalidOperationException("AsyncOperation failed to start.");
            }

            // Continuously publish loading progress
            while (!asyncOperation.isDone)
            {
                // Check for cancellation during the load
                cancellationToken.ThrowIfCancellationRequested();

                if (GameSceneManagerEventAggregator != null)
                {
                    // Publish the progress of the loading process
                    GameSceneManagerEventAggregator.Publish(new SceneLoadingProgressEvent
                        { Value = default, Progress = asyncOperation.progress });
                }


                await Task.Yield(); // Yield control back to the Unity engine to avoid blocking the main thread
            }

            if (GameSceneManagerEventAggregator != null)
            {
                // Now that the scene is loaded, get the actual scene object
                var loadedScene = GetSceneFromIdentifier(sceneIdentifier);
                // Publish event that the scene has finished loading
                GameSceneManagerEventAggregator.Publish(new SceneLoadingCompletedEvent { Value = loadedScene });
            }
        }


        private static async void LoadSceneAsync(object sceneIdentifier, LoadSceneParameters loadSceneParameters,
            CancellationToken cancellationToken = default)
        {
            try
            {
                GameSceneManagerEventAggregator.Publish(new SceneLoadingStartedEvent { Value = default });


                // Load the scene asynchronously and track the progress
                await ProcessLoadSceneAsync(sceneIdentifier, loadSceneParameters, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                GameSceneManagerEventAggregator.Publish(new SceneLoadingCanceledEvent { Value = default });
            }
        }
    }
}