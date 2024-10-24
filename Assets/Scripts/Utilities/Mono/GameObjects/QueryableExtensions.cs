using System.Collections.Generic;
using System.Linq;
using DOTSRTS.SceneManagement.Mono;
using DOTSRTS.Utilities.Mono.Transforms;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DOTSRTS.Utilities.Mono.GameObjects
{
    public static class QueryableExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this Query<T> queryable) => queryable.AsEnumerable();
        public static T[] ToArray<T>(this Query<T> queryable) => ToEnumerable(queryable).ToArray();
        public static List<T> ToList<T>(this Query<T> queryable) => ToEnumerable(queryable).ToList();

        // Find objects of type T in the scene
        public static Query<T> FindObjectsOfType<T>(
            this Query<T> query,
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude,
            FindObjectsSortMode findObjectsSortMode = FindObjectsSortMode.InstanceID) where T : Object
        {
            var objects = Object.FindObjectsByType<T>(findObjectsInactive, findObjectsSortMode);
            query = new Query<T>(objects);
            return query;
        }

        // Find interfaces of type T in the scene
        public static Query<T> FindInterfacesOfType<T>(
            this Query<T> query,
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude,
            FindObjectsSortMode findObjectsSortMode = FindObjectsSortMode.InstanceID) where T : class
        {
            var interfaces = TransformTools.FindAllInterfacesInScene<T>(findObjectsInactive, findObjectsSortMode);
            query = new Query<T>(interfaces);
            return query;
        }

        // Find objects of type T within a specific subscene
        public static Query<T> FindObjectsOfType<T>(this Query<T> query, SubScene subScene) where T : Component
        {
            var componentsInSubScene = SubSceneManagerTools.GetAllComponentsInSubScene<T>(subScene);
            query = new Query<T>(componentsInSubScene);
            return query;
        }
        public static Query<T> FindObjectsOfType<T>(this Query<T> query, Scene scene) where T : Component
        {
            // Ensure the scene is loaded
            if (!scene.isLoaded)
            {
                Debug.LogWarning("Scene is not loaded.");
                return query;
            }

            var componentsInScene = new List<T>();

            // Iterate over all root GameObjects in the scene
            foreach (var rootObject in scene.GetRootGameObjects())
            {
                // Get all components of type T in each root GameObject and its children
                componentsInScene.AddRange(rootObject.GetComponentsInChildren<T>(true));
            }

            // Create and return a new Query<T> with the found components
            query = new Query<T>(componentsInScene);
            return query;
        }

        // Find interfaces of type T within a specific subscene
        public static Query<T> FindInterfacesOfType<T>(this Query<T> query, SubScene subScene) where T : class
        {
            var interfacesInSubScene = SubSceneManagerTools.GetAllInterfacesInSubScene<T>(subScene);
            query = new Query<T>(interfacesInSubScene);
            return query;
        }

        public static Query<T> FindInterfacesOfType<T>(this Query<T> query, Scene subScene) where T : class
        {
            var interfacesInSubScene = SubSceneManagerTools.GetAllInterfacesInScene<T>(subScene);
            query = new Query<T>(interfacesInSubScene);
            return query;
        }
 
    }
}