using System;
using System.Collections.Generic;
using DOTSRTS.Utilities.Mono.Transforms;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;
using Object = UnityEngine.Object;

namespace DOTSRTS.Utilities.Mono.GameObjects
{
    public static class GameObjectTools
    {
        public static bool HasParent(this GameObject gameObject) => gameObject.transform.HasParent();
        public static float3 Position(this GameObject gameObject) => gameObject.transform.Position();
        public static quaternion Rotation(this GameObject gameObject) => gameObject.transform.Rotation();

#if UNITY_EDITOR
        public static Hash128 GetAssetGUID(this GameObject currentPrefab)
        {
            // Draw the object field for selecting a prefab
            // Get the Asset GUID if the selected object is a valid prefab
            if (currentPrefab == null) return default;
            string assetPath = AssetDatabase.GetAssetPath(currentPrefab);
            string assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
            return new Hash128(assetGUID);

            // Return null if no prefab is selected
        }


        public static void FocusOnGameObject(GameObject gameObject, bool instant = true)
        {
            // Check if the GameObject has a Renderer component

            // If it doesn't have a Renderer, focus on its Transform position
            // var transform = gameObject.transform;
            // var position = transform.position;
            // var bounds = new Bounds(position, Vector3.one); // Create a small bounds around the position
            // SceneView.lastActiveSceneView.Frame(bounds, instant);
            Selection.activeGameObject = gameObject;
            // Optionally, ping the GameObject in the hierarchy
             EditorGUIUtility.PingObject(gameObject);
        }

#endif


        public static void RemoveComponent<T>(this Component target) where T : Component
        {
            var component = target.GetComponent<T>();
            if (component == null) return;
#if UNITY_EDITOR

            if (Application.isPlaying)
            {
                Object.Destroy(component);
            }
            else
            {
                Object.DestroyImmediate(component);
            } // Use DestroyImmediate in the editor if needed
#else
            Object.Destroy(component);

#endif
        }

        public static GameObject FindChildByName(this GameObject parent, string childName)
        {
            if (parent == null)
            {
                Debug.LogError("Parent GameObject is null.");
                return null;
            }

            // Use a queue to perform a breadth-first search
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(parent.transform);

            while (queue.Count > 0)
            {
                Transform current = queue.Dequeue();

                if (current.name.Equals(childName))
                {
                    return current.gameObject;
                }

                // Enqueue all children
                foreach (Transform child in current)
                {
                    queue.Enqueue(child);
                }
            }

            // If no child is found with the given name
            return null;
        }
        
        public static T[] GetAllComponentsInHierarchy<T>(this GameObject gameObject,bool includeInActive = true) where T : Component
        {
            if (gameObject == null)
            {
                Debug.LogWarning("GameObject is null.");
                return Array.Empty<T>();
            }

            // Use a List to collect all components
            var componentsList = new List<T>();

            // Get the component on the root GameObject, if it exists
            var component = gameObject.GetComponent<T>();
            if (component != null)
            {
                componentsList.Add(component);
            }

            // Get components from all child objects
            var childComponents = gameObject.GetComponentsInChildren<T>(includeInActive);
            componentsList.AddRange(childComponents);

            return componentsList.ToArray();
        }
    }
}