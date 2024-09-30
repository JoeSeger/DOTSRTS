using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

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
        /// <param name="instance"></param>
        /// <param name="lockObject">The object to lock for thread safety.</param>
        /// <returns>The singleton instance.</returns>
        public static T ScriptableObjectInstance<T>(T instance,object lockObject) where T : ScriptableObject
        {
            if (instance != null)
            {
                return instance;
            }

            lock (lockObject)
            {
                if (instance != null) return instance;
                instance = LoadInstance<T>();
                Thread.MemoryBarrier();
            }

            return instance;
        }

        public static T LoadInstance<T>() where T : ScriptableObject
        {
            var assets = Resources.LoadAll<T>("");
            if (assets.Length > 0)
            {
                if (assets.Length > 1)
                {
                    Debug.LogWarning(
                        $"SingletonScriptableObject: Multiple instances of {typeof(T)} found in Resources. Using the first one found.");
                }
                return assets[0];
            }

            Debug.LogWarning($"SingletonScriptableObject: No instance of {typeof(T)} found in Resources. Creating a new instance.");
            return CreateScriptableObject<T>();
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
            // Ensure the folder path starts with "Assets/"
            if (!folderPath.StartsWith("Assets/"))
            {
                folderPath = "Assets/" + folderPath;
            }

            // Ensure the folder path contains "Resources/ScriptableObjects"
            if (!folderPath.Contains("Resources/ScriptableObjects"))
            {
                folderPath = Path.Combine(folderPath, "Resources/ScriptableObjects");
            }

            // Create the folder if it doesn't exist
            CreateFolderIfNotExists(folderPath);

            // Generate a unique path for the asset
            var assetPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{assetType.Name}.asset");

            // Create the asset in the specified folder
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
