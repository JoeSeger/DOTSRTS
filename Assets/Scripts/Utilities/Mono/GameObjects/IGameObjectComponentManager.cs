using System;
using System.Collections.Generic;
using System.Linq;
using DOTSRTS.SceneManagement.Mono;
using DOTSRTS.Utilities.Mono.Singletons;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace DOTSRTS.Utilities.Mono.GameObjects
{
    public class IGameObjectComponentManager : SerializedMonoBehaviour
    {
        [OdinSerialize] private List<IGameObjectComponent> _gameObjects = new();

        public event Action<IGameObjectComponent> OnGameObjectAdded;

        public event Action<IGameObjectComponent> OnGameObjectRemove;
        
        // Expose Query<T> for external querying
        public Query<IGameObjectComponent> Query => new(_gameObjects);

        private void OnValidate()
        {
            _gameObjects = SubSceneManagerTools.GetAllInterfacesInScene<IGameObjectComponent>(gameObject.scene);
        }

        public void AddGameObject(IGameObjectComponent iGameObject)
        {
            if (Query.Contains(iGameObject)) return; // Use the Query for checking existence
            _gameObjects.Add(iGameObject);
            OnGameObjectAdded?.Invoke(iGameObject);
        }

        public void RemoveGameObject(IGameObjectComponent iGameObject)
        {
            if (!Query.Contains(iGameObject)) return; // Use the Query for checking existence
            _gameObjects.Remove(iGameObject);
            OnGameObjectRemove?.Invoke(iGameObject);
        }

        public GameObject CreateGameObject(params Type[] components)
        {
            var newGameObject = new GameObject("NewGameObject", GetAddingTypes(components));
            foreach (var obj in newGameObject.GetComponents<IGameObjectComponent>())
            {
                AddGameObject(obj);
            }

            return newGameObject;
        }

        public GameObject CreateGameObject(string gameObjectName, params Type[] components)
        {
            var newGameObject = CreateGameObject(components);
            newGameObject.name = gameObjectName;
            return newGameObject;
        }

        public GameObject CreateGameObject(Transform parent, params Type[] components)
        {
            var newGameObject = CreateGameObject(components);
            newGameObject.transform.parent = parent;
            return newGameObject;
        }

        public void DestroyGameObject(GameObject destroyingGameObject)
        {
            foreach (var obj in destroyingGameObject.GetComponents<IDisposable>())
            {
                obj.Dispose();
            }
#if UNITY_EDITOR
            DestroyImmediate(destroyingGameObject);
#else
            Destroy(destroyingGameObject);
#endif
        }

        private static Type[] GetAddingTypes(params Type[] components)
            => components.Where(component => typeof(IGameObjectComponent).IsAssignableFrom(component)).ToArray();


        private void OnDestroy()
        {
            if (Query.IsCreated)
            {
                Query.Dispose();
            }
        }
    }
}