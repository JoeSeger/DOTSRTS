
using DOTSRTS.Utilities.DOTSGrid.Data;
using Unity.Mathematics;
using UnityEngine;

namespace DOTSRTS.Utilities.Mono
{
    public static class DebugExtensions
    {
        
         public static void DrawBounds(BoundsFloat3 bounds, Color color)
        {
            float3 min = bounds.Min;
            float3 max = bounds.Max;

            // Calculate the 8 corners of the bounds box
            var corners = new float3[8];
            corners[0] = new float3(min.x, min.y, min.z); // Bottom-left-front
            corners[1] = new float3(max.x, min.y, min.z); // Bottom-right-front
            corners[2] = new float3(max.x, min.y, max.z); // Bottom-right-back
            corners[3] = new float3(min.x, min.y, max.z); // Bottom-left-back
            corners[4] = new float3(min.x, max.y, min.z); // Top-left-front
            corners[5] = new float3(max.x, max.y, min.z); // Top-right-front
            corners[6] = new float3(max.x, max.y, max.z); // Top-right-back
            corners[7] = new float3(min.x, max.y, max.z); // Top-left-back

            // Draw bottom face
            Debug.DrawLine(corners[0], corners[1], color);
            Debug.DrawLine(corners[1], corners[2], color);
            Debug.DrawLine(corners[2], corners[3], color);
            Debug.DrawLine(corners[3], corners[0], color);

            // Draw top face
            Debug.DrawLine(corners[4], corners[5], color);
            Debug.DrawLine(corners[5], corners[6], color);
            Debug.DrawLine(corners[6], corners[7], color);
            Debug.DrawLine(corners[7], corners[4], color);

            // Draw vertical lines connecting top and bottom faces
            Debug.DrawLine(corners[0], corners[4], color);
            Debug.DrawLine(corners[1], corners[5], color);
            Debug.DrawLine(corners[2], corners[6], color);
            Debug.DrawLine(corners[3], corners[7], color);
        }
    
      
        /// <summary>
        /// Draws a wireframe plane using Debug.DrawLine.
        /// </summary>
        public static void DrawWirePlane(float3 center, float2 size, Color color)
        {
            var halfSize = new float3(size.x / 2f, 0f, size.y / 2f);
            var topLeft = center + new float3(-halfSize.x, 0f, halfSize.z);
            var topRight = center + new float3(halfSize.x, 0f, halfSize.z);
            var bottomLeft = center + new float3(-halfSize.x, 0f, -halfSize.z);
            var bottomRight = center + new float3(halfSize.x, 0f, -halfSize.z);

            Debug.DrawLine(topLeft, topRight, color);
            Debug.DrawLine(topRight, bottomRight, color);
            Debug.DrawLine(bottomRight, bottomLeft, color);
            Debug.DrawLine(bottomLeft, topLeft, color);
        }

        /// <summary>
        /// Draws a wireframe sphere using Debug.DrawLine.
        /// </summary>
        public static void DrawWireSphere(float3 center, float radius, Color color)
        {
            var segments = 36;
            var angleStep = 360f / segments;

            for (var i = 0; i < segments; i++)
            {
                var angle1 = math.radians(i * angleStep);
                var angle2 = math.radians((i + 1) * angleStep);

                var point1XZ = center + new float3(math.cos(angle1) * radius, 0, math.sin(angle1) * radius);
                var point2XZ = center + new float3(math.cos(angle2) * radius, 0, math.sin(angle2) * radius);
                Debug.DrawLine(point1XZ, point2XZ, color);

                var point1XY = center + new float3(math.cos(angle1) * radius, math.sin(angle1) * radius, 0);
                var point2XY = center + new float3(math.cos(angle2) * radius, math.sin(angle2) * radius, 0);
                Debug.DrawLine(point1XY, point2XY, color);

                var point1YZ = center + new float3(0, math.cos(angle1) * radius, math.sin(angle1) * radius);
                var point2YZ = center + new float3(0, math.cos(angle2) * radius, math.sin(angle2) * radius);
                Debug.DrawLine(point1YZ, point2YZ, color);
            }
        }
       
        public static void DrawWireBox(float3 center, float3 size, Color color)
        {
            var halfSize = size / 2f;

            var corners = new float3[8];
            corners[0] = center + new float3(-halfSize.x, -halfSize.y, -halfSize.z);
            corners[1] = center + new float3(halfSize.x, -halfSize.y, -halfSize.z);
            corners[2] = center + new float3(halfSize.x, -halfSize.y, halfSize.z);
            corners[3] = center + new float3(-halfSize.x, -halfSize.y, halfSize.z);
            corners[4] = center + new float3(-halfSize.x, halfSize.y, -halfSize.z);
            corners[5] = center + new float3(halfSize.x, halfSize.y, -halfSize.z);
            corners[6] = center + new float3(halfSize.x, halfSize.y, halfSize.z);
            corners[7] = center + new float3(-halfSize.x, halfSize.y, halfSize.z);

            // Bottom face
            Debug.DrawLine(corners[0], corners[1], color);
            Debug.DrawLine(corners[1], corners[2], color);
            Debug.DrawLine(corners[2], corners[3], color);
            Debug.DrawLine(corners[3], corners[0], color);

            // Top face
            Debug.DrawLine(corners[4], corners[5], color);
            Debug.DrawLine(corners[5], corners[6], color);
            Debug.DrawLine(corners[6], corners[7], color);
            Debug.DrawLine(corners[7], corners[4], color);

            // Vertical lines
            Debug.DrawLine(corners[0], corners[4], color);
            Debug.DrawLine(corners[1], corners[5], color);
            Debug.DrawLine(corners[2], corners[6], color);
            Debug.DrawLine(corners[3], corners[7], color);
        }

        /// <summary>
        /// Draws a wireframe cylinder using Debug.DrawLine.
        /// </summary>
        public static void DrawWireCylinder(float3 center, float radius, float height, Color color)
        {
            var segments = 36;
            var angleStep = 360f / segments;

            var topCenter = center + new float3(0, height / 2f, 0);
            var bottomCenter = center - new float3(0, height / 2f, 0);

            // Draw circles on the top and bottom
            for (var i = 0; i < segments; i++)
            {
                var angle1 = math.radians(i * angleStep);
                var angle2 = math.radians((i + 1) * angleStep);

                var point1Top = topCenter + new float3(math.cos(angle1) * radius, 0, math.sin(angle1) * radius);
                var point2Top = topCenter + new float3(math.cos(angle2) * radius, 0, math.sin(angle2) * radius);
                var point1Bottom = bottomCenter + new float3(math.cos(angle1) * radius, 0, math.sin(angle1) * radius);
                var point2Bottom = bottomCenter + new float3(math.cos(angle2) * radius, 0, math.sin(angle2) * radius);

                Debug.DrawLine(point1Top, point2Top, color);
                Debug.DrawLine(point1Bottom, point2Bottom, color);

                // Draw vertical lines between top and bottom
                Debug.DrawLine(point1Top, point1Bottom, color);
            }
        }
    }
}
