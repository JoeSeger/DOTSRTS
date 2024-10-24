using Unity.Mathematics;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using DOTSRTS.Utilities.Mono;

namespace DOTSRTS.Utilities.DOTSGrid.Data
{
    public static class BoundFloatTools
    {
        public static bool IsMouseOverBounds(this BoundsFloat3 bounds, float2 screenPosition, out float3 distance)
        {
#if UNITY_EDITOR

            return bounds.IsMouseOverBounds(screenPosition, SceneView.lastActiveSceneView.camera, out distance);
#else
            return bounds.IsMouseOverBounds(screenPosition,Camera.main, out distance);
#endif
        }


        public static bool IsMouseOverBounds(this BoundsFloat3 bounds, float2 screenPosition, Camera camera,
            out float3 distance)
        {
            distance = default;

            if (camera == null) return false;
            // Generate a ray from the camera using the last known mouse position
            Ray ray = camera.ScreenPointToRay(screenPosition);

            // Visualize the ray in the SceneView

            // Check if the ray intersects with the grid bounds
            bool hit = BoundsFloat3.IntersectRayAABB(ray, bounds, out var rayDistance);
            distance = rayDistance;
            return hit;
        }

        public static bool IsMouseOverBounds(this BoundsFloat3 bounds, float2 screenPosition)
        {
            return bounds.IsMouseOverBounds(screenPosition, out _);
        }
    }
}