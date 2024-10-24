using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Scenes;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hash128 = Unity.Entities.Hash128;

namespace DOTSRTS.SceneManagement.Mono
{
    public static class SubSceneManagerTools
    {
        public static List<SubScene> AllSubScenes => GetAllSubScenesInRootScene();

        public static bool AllScenesLoaded => AllSubScenes.All(subScene => subScene.IsLoaded);

        public static bool SceneIsLoaded(int sceneID) => AllSubScenes[sceneID].IsLoaded;

        public static bool SceneIsLoaded(Hash128 sceneGuid)
        {
            return GetSubScene(sceneGuid).IsLoaded;
        }

        public static SubScene GetSubScene(Hash128 sceneGuid) =>AllSubScenes.FirstOrDefault(subScene => subScene.SceneGUID == sceneGuid); 
        public static SubScene GetSubScene(int buildIndex) =>AllSubScenes.FirstOrDefault(subScene => subScene.EditingScene.buildIndex == buildIndex); 
     
#if UNITY_EDITOR
        public static void EditorRefresh()
        {
            foreach (var subScene in AllSubScenes)
            {
                var newSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(subScene.EditableScenePath);
                subScene.SceneAsset = newSceneAsset;
                EditorUtility.SetDirty(subScene);
            }

            AssetDatabase.SaveAssets();
        }
#endif
        public static List<SubScene> GetAllSubScenesInRootScene()
        {
            var subScenes = new List<SubScene>();
        
            // Get the active scene
            var activeScene = SceneManager.GetActiveScene();

            // Retrieve all root game objects in the active scene
            var rootGameObjects = activeScene.GetRootGameObjects();

            // Iterate over root game objects to find SubScene components
            foreach (var rootGameObject in rootGameObjects)
            {
                var subSceneComponents = rootGameObject.GetComponentsInChildren<SubScene>(true); // Include inactive SubScenes
                subScenes.AddRange(subSceneComponents);
            }

            return subScenes;
        }
        public static SubScene GetSubSceneFromComponent(Component component) =>  
            AllSubScenes.FirstOrDefault(subScene => subScene.EditingScene == component.gameObject.scene);
        
        public static bool IsSubSceneLoaded(Hash128 sceneGuid, WorldUnmanaged world)
        {
            // Get the scene entity using its GUID
            Entity sceneEntity = SceneSystem.GetSceneEntity(world, sceneGuid);

            // Check if the scene entity is valid
            if (sceneEntity == Entity.Null)
            {
                Debug.Log("Scene entity is not valid, scene is not loaded.");
                return false;
            }

            // Check if the scene is loaded
            bool isLoaded = SceneSystem.IsSceneLoaded(world, sceneEntity);

            Debug.Log($"Scene {sceneGuid} loaded status: {isLoaded}");

            return isLoaded;
        }
           /// <summary>
    /// Get all components of type T in a given subscene, including from root and child GameObjects.
    /// </summary>
    /// <typeparam name="T">The type of component to search for (must be a Component).</typeparam>
    /// <param name="subScene">The SubScene component representing the subscene.</param>
    /// <returns>List of components of type T found in the subscene.</returns>
    public static List<T> GetAllComponentsInSubScene<T>(SubScene subScene) where T : Component
    {
        var components = new List<T>();

        if (subScene == null || !subScene.IsLoaded)
        {
            Debug.LogWarning("SubScene is null or not loaded.");
            return components;
        }

        // Get the Scene object associated with the SubScene
        Scene subSceneUnityScene = subScene.EditingScene;

        if (!subSceneUnityScene.isLoaded)
        {
            Debug.LogWarning("SubScene Unity Scene is not loaded.");
            return components;
        }

        // Iterate through all root GameObjects in the subscene
        foreach (var rootObject in subSceneUnityScene.GetRootGameObjects())
        {
            // Add all components of type T from the root object and its children
            GetComponentsInGameObjectAndChildren(rootObject, components);
        }

        return components;
    }

    /// <summary>
    /// Recursively find components of type T in the given GameObject and its children.
    /// </summary>
    /// <typeparam name="T">The type of component to search for.</typeparam>
    /// <param name="gameObject">The root GameObject to start the search.</param>
    /// <param name="components">List to which found components are added.</param>
    private static void GetComponentsInGameObjectAndChildren<T>(GameObject gameObject, List<T> components) where T : Component
    {
        // Get all components of type T on this GameObject
        var foundComponents = gameObject.GetComponents<T>();
        components.AddRange(foundComponents);

        // Recursively search in children
        foreach (Transform child in gameObject.transform)
        {
            GetComponentsInGameObjectAndChildren(child.gameObject, components);
        }
    }

        
        /// <summary>
        /// Get all GameObjects in a given subscene at runtime.
        /// </summary>
        /// <param name="subScene">The SubScene component representing the subscene.</param>
        /// <returns>List of GameObjects in the subscene.</returns>
        public static List<GameObject> GetAllGameObjectsInSubScene(SubScene subScene)
        {
            var gameObjects = new List<GameObject>();

            if (subScene == null || !subScene.IsLoaded) 
            {
                Debug.LogWarning("SubScene is null or not loaded.");
                return gameObjects;
            }

            // Get the Scene object associated with the SubScene
            Scene subSceneUnityScene = subScene.EditingScene;

            if (!subSceneUnityScene.isLoaded) 
            {
                Debug.LogWarning("SubScene Unity Scene is not loaded.");
                return gameObjects;
            }

            // Iterate through all GameObjects in the subscene
            foreach (var rootObject in subSceneUnityScene.GetRootGameObjects())
            {
                gameObjects.Add(rootObject);

                // Optionally, you can add all child GameObjects as well
                GetChildGameObjects(rootObject, gameObjects);
            }

            return gameObjects;
        }

        /// <summary>
        /// Recursively adds child GameObjects to the list.
        /// </summary>
        /// <param name="parent">Parent GameObject</param>
        /// <param name="gameObjects">List of GameObjects to add to</param>
        private static void GetChildGameObjects(GameObject parent, List<GameObject> gameObjects)
        {
            foreach (Transform child in parent.transform)
            {
                gameObjects.Add(child.gameObject);
                GetChildGameObjects(child.gameObject, gameObjects);
            }
        }
        
        /// <summary>
    /// Get all components in a given subscene that implement the specified interface at runtime.
    /// </summary>
    /// <typeparam name="T">The interface type to search for (e.g., IGameObjectComponent)</typeparam>
    /// <param name="subScene">The SubScene component representing the subscene.</param>
    /// <returns>List of components that implement the specified interface.</returns>
    public static List<T> GetAllInterfacesInSubScene<T>(SubScene subScene) where T : class
    {
        var componentsWithInterface = new List<T>();

        if (subScene == null || !subScene.IsLoaded)
        {
            Debug.LogWarning("SubScene is null or not loaded.");
            return componentsWithInterface;
        }

        // Get the Scene object associated with the SubScene
        Scene subSceneUnityScene = subScene.EditingScene;

        if (!subSceneUnityScene.isLoaded)
        {
            Debug.LogWarning("SubScene Unity Scene is not loaded.");
            return componentsWithInterface;
        }

        // Iterate through all GameObjects in the subscene
        foreach (var rootObject in subSceneUnityScene.GetRootGameObjects())
        {
            // Find all components on the root object and its children that implement the specified interface
            FindInterfacesInGameObjectHierarchy(rootObject, componentsWithInterface);
        }

        return componentsWithInterface;
    }
        public static List<T> GetAllInterfacesInScene<T>(Scene subScene) where T : class
        {
            var componentsWithInterface = new List<T>();


            if (!subScene.isLoaded)
            {
                Debug.LogWarning("SubScene Unity Scene is not loaded.");
                return componentsWithInterface;
            }

            // Iterate through all GameObjects in the subscene
            foreach (var rootObject in subScene.GetRootGameObjects())
            {
                // Find all components on the root object and its children that implement the specified interface
                FindInterfacesInGameObjectHierarchy(rootObject, componentsWithInterface);
            }

            return componentsWithInterface;
        }

    /// <summary>
    /// Recursively finds components in the GameObject hierarchy that implement the specified interface.
    /// </summary>
    /// <typeparam name="T">The interface type to search for</typeparam>
    /// <param name="gameObject">The parent GameObject</param>
    /// <param name="componentsWithInterface">List of found components to add to</param>
    private static void FindInterfacesInGameObjectHierarchy<T>(GameObject gameObject, List<T> componentsWithInterface) where T : class
    {
        // Get all components on the current GameObject that implement the interface T
        var components = gameObject.GetComponents<T>();
        componentsWithInterface.AddRange(components);

        // Recursively search in children
        foreach (Transform child in gameObject.transform)
        {
            FindInterfacesInGameObjectHierarchy(child.gameObject, componentsWithInterface);
        }
    }
    }
}