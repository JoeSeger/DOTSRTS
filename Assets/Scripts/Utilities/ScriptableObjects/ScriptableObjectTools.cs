using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace DOTSRTS.Utilities.ScriptableObjects
{
    public static class ScriptableObjectTools
    {
        /// <summary>
        /// Creates a ScriptableObject of the specified type in the given folder path.
        /// Works in both Editor and Runtime.
        /// </summary>
        /// <typeparam name="T">The type of ScriptableObject to create.</typeparam>
        /// <param name="folderPath">The folder path where the ScriptableObject will be created (Editor only).</param>
        /// <returns>The created ScriptableObject.</returns>
        public static T CreateScriptableObject<T>(string folderPath = null) where T : ScriptableObject
        {
            return (T)CreateScriptableObject(typeof(T), folderPath);
        }

        /// <summary>
        /// Returns a thread-safe singleton instance of the ScriptableObject.
        /// </summary>
        /// <typeparam name="T">The type of ScriptableObject.</typeparam>
        /// <param name="instance">Reference to the singleton instance.</param>
        /// <param name="lockObject">The object to lock for thread safety.</param>
        /// <returns>The singleton instance.</returns>
        public static T ScriptableObjectInstance<T>(ref T instance, object lockObject) where T : ScriptableObject
        {
            if (instance != null)
            {
                return instance;
            }

            lock (lockObject)
            {
                if (instance == null)
                {
                    instance = LoadInstance<T>();
                    Thread.MemoryBarrier(); // Ensures changes are visible to all threads
                }
            }

            return instance;
        }

        /// <summary>
        /// Loads an existing instance of a ScriptableObject from the Resources folder.
        /// If no instance is found, a new one is created.
        /// </summary>
        /// <typeparam name="T">The type of ScriptableObject.</typeparam>
        /// <returns>The loaded or newly created ScriptableObject instance.</returns>
        public static T LoadInstance<T>() where T : ScriptableObject
        {
            var assets = Resources.LoadAll<T>("");

            if (assets.Length > 0)
            {
                if (assets.Length > 1)
                {
                    Debug.LogWarning($"Multiple instances of {typeof(T)} found in Resources. Using the first one.");
                }
                return assets[0];
            }

            Debug.LogWarning($"No instance of {typeof(T)} found in Resources. Creating a new instance.");
            return CreateScriptableObject<T>("Assets/Resources/ScriptableObjects");
        }

        /// <summary>
        /// Creates a ScriptableObject of the specified type in the given folder path.
        /// Works in both Editor and Runtime.
        /// </summary>
        /// <param name="assetType">The type of ScriptableObject to create.</param>
        /// <param name="folderPath">The folder path where the ScriptableObject will be created (Editor only).</param>
        /// <returns>The created ScriptableObject.</returns>
        public static ScriptableObject CreateScriptableObject(System.Type assetType, string folderPath)
        {
            var asset = ScriptableObject.CreateInstance(assetType);

#if UNITY_EDITOR
            if (string.IsNullOrEmpty(folderPath))
            {
                folderPath = "Assets/Resources/ScriptableObjects";
            }

            if (!folderPath.StartsWith("Assets/"))
            {
                folderPath = "Assets/" + folderPath;
            }

            if (!folderPath.Contains("Resources/ScriptableObjects"))
            {
                folderPath = Path.Combine(folderPath, "Resources/ScriptableObjects");
            }

            CreateFolderIfNotExists(folderPath);

            var assetPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{assetType.Name}.asset");
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();

            Debug.Log($"ScriptableObject of type {assetType} created at {assetPath}");
#endif
            return asset;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Creates the folder if it doesn't exist.
        /// </summary>
        /// <param name="folderPath">The folder path to create.</param>
        private static void CreateFolderIfNotExists(string folderPath)
        {
            var folders = folderPath.Split('/');
            var currentPath = folders[0];

            for (var i = 1; i < folders.Length; i++)
            {
                var newPath = Path.Combine(currentPath, folders[i]);

                if (!AssetDatabase.IsValidFolder(newPath))
                {
                    AssetDatabase.CreateFolder(currentPath, folders[i]);
                }

                currentPath = newPath;
            }
        }
#endif
    }
}
