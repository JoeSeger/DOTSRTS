using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DOTSRTS.Utilities.Mono.Transforms
{
    public static class TransformTools
    {
        public static bool HasParent(this Transform transform) => transform.parent != null;

        public static float3 Position(this Transform transform) =>
            transform.HasParent() ? transform.localPosition : transform.position;

        public static quaternion Rotation(this Transform transform) =>
            transform.HasParent() ? transform.localRotation : transform.rotation;
        
        
         public static List<object> FindAllInterfacesInScene(Type interfaceType,FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include,FindObjectsSortMode findObjectsSortMode = FindObjectsSortMode.None)
        {
            if (!interfaceType.IsInterface)
            {
                Debug.LogError($"The provided type {interfaceType.Name} is not an interface.");
                return new List<object>();
            }

            var result = new List<object>();

            // Iterate through all GameObjects in the scene
            foreach (var transform in Object.FindObjectsByType<Transform>(findObjectsInactive,findObjectsSortMode))
            {
                // Get all components on the current GameObject
                var components = transform.GetComponents<Component>();

                // Check each component to see if it implements the specified interface
                result.AddRange(components.Where(component => component != null && interfaceType.IsInstanceOfType(component)).Cast<object>());
            }

            return result;
        }
        public static List<T> FindAllInterfacesInScene<T>(FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include,FindObjectsSortMode findObjectsSortMode = FindObjectsSortMode.InstanceID) where T : class
        {
            var result = new List<T>();

            // Iterate through all GameObjects in the scene
            foreach (var transform in Object.FindObjectsByType<Transform>(findObjectsInactive,findObjectsSortMode))
            {
                // Get all components on the current GameObject
                var components = transform.GetComponents<T>();
                // Check each component to see if it implements the interface T
                result.AddRange(components.Where(component => component != null));
            }

            return result;
        }

        public static LocalTransform ConvertToLocalTransform(this Transform transform)
        {
            var scale = math.max(transform.localScale.x, math.max(transform.localScale.y, transform.localScale.z));
            return LocalTransform.FromPositionRotationScale(transform.position, transform.rotation, scale);
        }

        public static bool HasComponent<T>(this Transform transform) where T : Component =>
            transform.TryGetComponent<T>(out _);

        public static bool TryGetComponent<T>(this Transform transform, out T component) where T : Component
        {
            component = transform.GetComponent<T>();
            return component != null;
        }

        public static T GetOrAddComponent<T>(this Transform transform) where T : Component =>
            transform.TryGetComponent(out T foundComponent) ? foundComponent : transform.AddComponent<T>();
        
      
        public static IEnumerable<T> GetAllComponentsInChildren<T>(this Transform parent, bool includeActive = true) where T : class
        {
            return parent.GetAllComponentsInChildren<T>(typeof(T), includeActive);
        }

        public static Component[] GetAllComponentsInChildren(this Transform parent, Type type, bool includeInactive = true)
        {
            if (parent != null)
                return parent.GetComponentsInChildren<Transform>(includeInactive)
                    .Select(child => child.GetComponent(type)).Where(component => component != null).ToArray();
            Debug.LogError("Parent transform is null");
            return null;

        }
        public static IEnumerable<T> GetAllComponentsInChildren<T>(this Transform parent,Type type, bool includeActive = true) where T : class
        {
            if (parent == null)
            {
                Debug.LogError("Parent transform is null");
                yield break;
            }

            var allChildren = parent.GetAllChildren(includeActive);
            var returningInterface = new List<T>();

            foreach (var child in allChildren)
            {
                if (child == null)
                {
                    Debug.LogWarning("Child transform is null");
                    continue;
                }

                if (child.GetComponent(type) is not T gettingInterface) continue;
                Debug.Log($"Found {typeof(T).Name} on {child.name}");
                returningInterface.Add(gettingInterface);
            }

            foreach (var item in returningInterface)
            {
                yield return item;
            }
        }
        public static int GetChildIndex(this Transform child)
        {
            var parent = child.parent;
            if (parent == null || child == null) return -1;

            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i) == child)
                {
                    return i;
                }
            }

            return -1; // Child not found
        }

        public static LocalTransform ToLocalTransform(this Transform transform, bool lossyScale = false) =>
            LocalTransform.FromPositionRotationScale(transform.position, transform.rotation,
                transform.Scale(lossyScale));


        public static float Scale(this Transform transform, bool lossyScale = false,float neatestValue = 10000f) => 
            lossyScale ? ConvertLossyScaleToFloat(transform.lossyScale,neatestValue) : ConvertLocalScaleToFloat(transform.localScale,neatestValue);

        public static float ConvertLocalScaleToFloat(float3 localScale,float neatestValue = 10000f)
        {
            float averageScale = (localScale.x + localScale.y + localScale.z) / 3f;
            return math.round(averageScale * neatestValue) / neatestValue;
        }

        public static float ConvertLossyScaleToFloat(float3 lossyScale,float neatestValue = 10000f)
        {
            float averageScale = math.max(math.max(lossyScale.x, lossyScale.y), lossyScale.z);
            return math.round(averageScale * neatestValue) / neatestValue;
        }
   

        public static IEnumerable<Transform> GetAllChildren(this Transform parent, bool includeActive = true)
        {
            var children = new List<Transform>();

            // Use a queue for BFS
            var queue = new Queue<Transform>();

            // Enqueue all children of the parent node
            foreach (Transform child in parent)
            {
                queue.Enqueue(child);
            }

            // Iterate through the queue
            while (queue.Count > 0)
            {
                // Dequeue the current node
                var currentNode = queue.Dequeue();

                // Check if the current node should be included based on its active state
                if (includeActive || currentNode.gameObject.activeInHierarchy)
                {
                    // Add the current node to the list
                    children.Add(currentNode);
                }

                // Enqueue all children of the current node
                foreach (Transform child in currentNode)
                {
                    queue.Enqueue(child);
                }
            }

            return children;
        }
    }
    
}