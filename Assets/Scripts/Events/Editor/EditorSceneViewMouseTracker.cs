// using DOTSRTS.Events.Controls;
// using DOTSRTS.Utilities.DOTSGrid.Data;
// using Unity.Mathematics;
// using UnityEditor;
// using UnityEngine;
//
// namespace DOTSRTS.Events.Editor
// {
//     [InitializeOnLoad]
//     public static class EditorSceneViewMouseTracker
//     {
//         private static Vector2 _lastKnownMousePosition;
//         
//         static EditorSceneViewMouseTracker()
//         {
//             // Subscribe to the SceneView's GUI event to capture events during the SceneView's rendering
//             EditorEventManager.EditorEventAggregator.Subscribe<OnMouseEventChanged>(OnMouseEventChanged);
//         }
//
//         private static void OnMouseEventChanged(OnMouseEventChanged args)
//         {
//             if (SceneView.lastActiveSceneView == null || SceneView.lastActiveSceneView.camera == null) return;
//             
//             // Convert GUI position to Screen position
//             _lastKnownMousePosition = HandleUtility.GUIPointToScreenPixelCoordinate(args.CurrentMousePosition);
//         }
//         
//         
//         // public static bool IsMouseOverBounds(BoundsFloat3 bounds,out float3 distance)
//         // {
//         //     distance = default;
//         //
//         //     // If no valid mouse position is available, return false
//         //     if (SceneView.lastActiveSceneView == null)
//         //     {
//         //         return false;
//         //     }
//         //
//         //     Camera sceneCamera = SceneView.lastActiveSceneView.camera;
//         //     if (sceneCamera == null) return false;
//         //
//         //     // Generate a ray from the camera using the last known mouse position
//         //     Ray ray = sceneCamera.ScreenPointToRay(_lastKnownMousePosition);
//         //
//         //     // Visualize the ray in the SceneView
//         //
//         //     // Check if the ray intersects with the grid bounds
//         //     bool hit = BoundsFloat3.IntersectRayAABB(ray, bounds, out var rayDistance);
//         //     distance = rayDistance;
//         //     return hit;
//         // }
//         //
//         // // Overload for checking bounds without needing the distance output
//         // public static bool IsMouseOverBounds(BoundsFloat3 bounds)
//         // {
//         //     return IsMouseOverBounds(bounds,out _);
//         // }
//         //
//         // // Gets the last known mouse position in the Scene View
//         // public static Vector2? GetLastKnownMousePosition()
//         // {
//         //     return _lastKnownMousePosition;
//         // }
//         
//         
//     }
// }