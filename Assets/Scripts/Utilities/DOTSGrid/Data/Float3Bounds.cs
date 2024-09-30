using Unity.Mathematics;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTSGrid.Data
{
    public struct Float3Bounds
    {
        private float3 m_Center;
        private float3 m_Extents;

        // Constructor
        public Float3Bounds(float3 center, float3 size)
        {
            m_Center = center;
            m_Extents = size * 0.5f;
        }

        // Center of the bounds
        public float3 Center
        {
            get => m_Center;
            set => m_Center = value;
        }

        // Size of the bounds (twice the extents)
        public float3 Size
        {
            get => m_Extents * 2f;
            set => m_Extents = value * 0.5f;
        }

        // Extents of the bounds (half of the size)
        public float3 Extents
        {
            get => m_Extents;
            set => m_Extents = value;
        }

        // Minimal point (center - extents)
        public float3 Min
        {
            get => m_Center - m_Extents;
            set => SetMinMax(value, Max);
        }

        // Maximal point (center + extents)
        public float3 Max
        {
            get => m_Center + m_Extents;
            set => SetMinMax(Min, value);
        }

        // Sets the bounds based on min and max values
        public void SetMinMax(float3 min, float3 max)
        {
            m_Extents = (max - min) * 0.5f;
            m_Center = min + m_Extents;
        }

        // Expands the bounds by a certain amount
        public void Expand(float amount)
        {
            amount *= 0.5f;
            m_Extents += new float3(amount, amount, amount);
        }

        // Expands the bounds by a certain float3 amount
        public void Expand(float3 amount)
        {
            m_Extents += amount * 0.5f;
        }

        // Encapsulate a point to adjust the bounds
        public void Encapsulate(float3 point)
        {
            SetMinMax(math.min(Min, point), math.max(Max, point));
        }

        // Encapsulate another bounds to adjust the bounds
        public void Encapsulate(Float3Bounds bounds)
        {
            Encapsulate(bounds.Min);
            Encapsulate(bounds.Max);
        }

        // Check if the bounds contain a point
        public bool Contains(float3 point)
        {
            return point.x >= Min.x && point.x <= Max.x &&
                   point.y >= Min.y && point.y <= Max.y &&
                   point.z >= Min.z && point.z <= Max.z;
        }

        // Returns the closest point inside the bounds
        public float3 ClosestPoint(float3 point)
        {
            return math.clamp(point, Min, Max);
        }

        // To string method for easier debugging
        public override string ToString()
        {
            return $"Center: {m_Center}, Extents: {m_Extents}";
        }

        // Check if the bounds intersect another bounds
        public bool Intersects(Float3Bounds bounds)
        {
            return Min.x <= bounds.Max.x && Max.x >= bounds.Min.x &&
                   Min.y <= bounds.Max.y && Max.y >= bounds.Min.y &&
                   Min.z <= bounds.Max.z && Max.z >= bounds.Min.z;
        }

        // Simulate injected IntersectRayAABB function
        public static bool IntersectRayAABB(Ray ray, Float3Bounds bounds, out float distance)
        {
            distance = 0;
            float tmin = (bounds.Min.x - ray.origin.x) / ray.direction.x;
            float tmax = (bounds.Max.x - ray.origin.x) / ray.direction.x;

            if (tmin > tmax) (tmin, tmax) = (tmax, tmin);

            float tymin = (bounds.Min.z - ray.origin.z) / ray.direction.z;
            float tymax = (bounds.Max.z - ray.origin.z) / ray.direction.z;

            if (tymin > tymax) (tymin, tymax) = (tymax, tymin);

            if ((tmin > tymax) || (tymin > tmax)) return false;

            if (tymin > tmin) tmin = tymin;
            if (tymax < tmax) tmax = tymax;

            float tzmin = (bounds.Min.y - ray.origin.y) / ray.direction.y;
            float tzmax = (bounds.Max.y - ray.origin.y) / ray.direction.y;

            if (tzmin > tzmax) (tzmin, tzmax) = (tzmax, tzmin);

            if ((tmin > tzmax) || (tzmin > tmax)) return false;

            if (tzmin > tmin) tmin = tzmin;
            if (tzmax < tmax) tmax = tzmax;

            distance = tmin;
            return true;
        }

        // Simulate injected ClosestPoint function
        public float3 ClosestPoint_Injected(ref float3 point)
        {
            return ClosestPoint(point);
        }

        // Simulate injected Contains function
        public bool Contains_Injected(ref float3 point)
        {
            return Contains(point);
        }

        // Simulate injected SqrDistance function
        public float SqrDistance_Injected(ref float3 point)
        {
            float3 closestPoint = ClosestPoint(point);
            return math.lengthsq(closestPoint - point);
        }

        // Simulate injected IntersectRayAABB function
        public static bool IntersectRayAABB_Injected(ref Ray ray, ref Float3Bounds bounds, out float distance)
        {
            return IntersectRayAABB(ray, bounds, out distance);
        }
    }
}
