using System;
using System.Collections.Generic;
using System.Linq;
using DOTSRTS.Utilities.DOTS.Editor;
using UnityEditor;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json;

public static class MemoryLeakDetector
{
    public class LeakedObjectInfo
    {
        public string Type { get; set; }
        public int InstanceCount { get; set; }
        public List<string> ObjectNames { get; set; }
    }

    // Find leaked objects
    public static string FindLeakedObjects(params Type[] trackedTypes)
    {
        var leakedObjects = new List<LeakedObjectInfo>();

        foreach (var type in trackedTypes)
        {
            // Ensure the type is valid and derived from UnityEngine.Object
            if (!typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                Debug.LogWarning($"{type.Name} is not derived from UnityEngine.Object and cannot be tracked.");
                continue;
            }

            if (type.IsGenericTypeDefinition)
            {
                Debug.LogWarning($"{type.Name} is a generic type definition and cannot be tracked.");
                continue;
            }

            // Find objects of the valid type
            UnityEngine.Object[] objects = Resources.FindObjectsOfTypeAll(type);

            if (objects == null || objects.Length == 0)
            {
                Debug.LogWarning($"No instances found for type {type.Name}.");
                continue;
            }

            var leakedObjectInfo = new LeakedObjectInfo
            {
                Type = type.Name,
                InstanceCount = objects.Length,
                ObjectNames = new List<string>()
            };

            // Collect names of the objects
            foreach (var obj in objects)
            {
                if (obj != null && obj.hideFlags == HideFlags.None)
                {
                    leakedObjectInfo.ObjectNames.Add(obj.name);
                }
            }

            if (leakedObjectInfo.ObjectNames.Count > 0)
            {
                leakedObjects.Add(leakedObjectInfo);
            }
        }

        // Serialize to pretty JSON
        return JsonConvert.SerializeObject(leakedObjects, Formatting.Indented);
    }

    // Menu item for viewing leaked objects
    [MenuItem("Tools/ViewLeakedObjectInfo")]
    public static void ViewLeakedObjectInfo()
    {
        // Assuming ProjectTools.GetAllTypesInProject() returns a list of types in the project
        var allProjectObjects = ProjectTools.GetAllTypesInProject();

        if (allProjectObjects == null)
        {
            Debug.LogWarning("No types found in the project.");
            return;
        }

        var getLeakedObjectsString = FindLeakedObjects(allProjectObjects.ToArray());
        Debug.Log($"Leaked Objects String: {getLeakedObjectsString}");
    }
}
