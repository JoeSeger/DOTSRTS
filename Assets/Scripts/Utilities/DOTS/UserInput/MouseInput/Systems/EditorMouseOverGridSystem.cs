using DOTSRTS.Utilities.DOTS.UserInput.MouseInput.Components;
using DOTSRTS.Utilities.DOTSGrid.Data;
using DOTSRTS.Utilities.Mono;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTS.UserInput.MouseInput.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.EntityProxy |
                       WorldSystemFilterFlags.EntityProxyPreview)]
    public partial class EditorMouseOverGridSystem : OnSceneGUISystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<SceneViewFocusTag>();
        }

        protected override void OnUpdate()
        {
            if (CurrentSceneView == null || CurrentSceneEvent == null) return;

            // Get the mouse position relative to the scene view
            Vector2 mousePosition = CurrentSceneEvent.mousePosition;

            // Convert the mouse position into a world ray
            Ray worldRay = HandleUtility.GUIPointToWorldRay(mousePosition);

            // Iterate through all grids and check if the mouse is over any of them
            foreach (var (gridComponent, entity) in SystemAPI.Query<RefRW<GridComponent>>().WithEntityAccess())
            {
                // Access the BlobAssetReference for the grid data
                var gridBlobRef = gridComponent.ValueRO.GridAsset;

                // Check if the BlobAssetReference is valid and created
                if (!gridBlobRef.IsCreated) continue;

                ref var gridData = ref gridBlobRef.Value;

                // Perform the mouse-over check using the grid's bounds
                if (IsMouseOverGrid(worldRay, ref gridData, out var hitPosition))
                {
                    gridComponent.ValueRW.MouseOver = true;
                    DebugExtensions.DrawWireSphere(hitPosition, 1f, Color.magenta);
                }
                else
                {
                    gridComponent.ValueRW.MouseOver = false;
                }
            }
        }

        // Method to check if the ray intersects the grid's world-space bounds
        public static bool IsMouseOverGrid(Ray worldRay, ref GridAssetBlob gridData, out float3 hitPosition)
        {
            hitPosition = float3.zero;

            // Use the Float3Bounds of the grid to perform an AABB intersection test
            var gridBounds = gridData.Bounds;

            // Perform ray-AABB intersection using the grid's bounds
            return RayIntersectsBounds(worldRay, gridBounds, out hitPosition);
        }

        // Ray-AABB intersection check with Float3Bounds
        public static bool RayIntersectsBounds(Ray ray, Float3Bounds bounds, out float3 hitPoint)
        {
            hitPoint = float3.zero;

            // Perform slab-based ray-bounds intersection
            float tMin = (bounds.Min.x - ray.origin.x) / ray.direction.x;
            float tMax = (bounds.Max.x - ray.origin.x) / ray.direction.x;

            if (tMin > tMax) (tMin, tMax) = (tMax, tMin);

            float tyMin = (bounds.Min.z - ray.origin.z) / ray.direction.z;
            float tyMax = (bounds.Max.z - ray.origin.z) / ray.direction.z;

            if (tyMin > tyMax) (tyMin, tyMax) = (tyMax, tyMin);

            if ((tMin > tyMax) || (tyMin > tMax))
                return false;

            if (tyMin > tMin)
                tMin = tyMin;

            if (tyMax < tMax)
                tMax = tyMax;

            float tzMin = (bounds.Min.y - ray.origin.y) / ray.direction.y;
            float tzMax = (bounds.Max.y - ray.origin.y) / ray.direction.y;

            if (tzMin > tzMax) (tzMin, tzMax) = (tzMax, tzMin);

            if ((tMin > tzMax) || (tzMin > tMax))
                return false;

            if (tzMin > tMin)
                tMin = tzMin;

            if (tzMax < tMax)
                tMax = tzMax;

            // The hit point is where the ray intersects the bounds
            hitPoint = ray.origin + ray.direction * tMin;
            return true;
        }
    }
}
