using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTSGrid.Data
{
    [Serializable]
    public struct BoundsFloat3
    {
        [SerializeField] private float3 center;
        [SerializeField] private float3 extents;

        // Constructor
        public BoundsFloat3(float3 center, float3 size)
        {
            this.center = center;
            extents = size * 0.5f;
        }

        // Center of the bounds
        public float3 Center
        {
            get => center;
            set => center = value;
        }

        // Size of the bounds (twice the extents)
        public float3 Size
        {
            get => extents * 2f;
            set => extents = value * 0.5f;
        }

        // Extents of the bounds (half of the size)
        public float3 Extents
        {
            get => extents;
            set => extents = value;
        }

        // Minimal point (center - extents)
        public float3 Min
        {
            get => center - extents;
            set => SetMinMax(value, Max);
        }

        // Maximal point (center + extents)
        public float3 Max
        {
            get => center + extents;
            set => SetMinMax(Min, value);
        }

        // Sets the bounds based on min and max values
        public void SetMinMax(float3 min, float3 max)
        {
            extents = (max - min) * 0.5f;
            center = min + extents;
        }

        // Expands the bounds by a certain amount
        public void Expand(float amount)
        {
            amount *= 0.5f;
            extents += new float3(amount, amount, amount);
        }

        // Expands the bounds by a certain float3 amount
        public void Expand(float3 amount)
        {
            extents += amount * 0.5f;
        }

        // Encapsulate a point to adjust the bounds
        public void Encapsulate(float3 point)
        {
            SetMinMax(math.min(Min, point), math.max(Max, point));
        }

        // Encapsulate another bounds to adjust the bounds
        public void Encapsulate(BoundsFloat3 bounds)
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

        // Check if the bounds intersect another bounds
        public bool Intersects(BoundsFloat3 bounds)
        {
            return Min.x <= bounds.Max.x && Max.x >= bounds.Min.x &&
                   Min.y <= bounds.Max.y && Max.y >= bounds.Min.y &&
                   Min.z <= bounds.Max.z && Max.z >= bounds.Min.z;
        }

        // Intersects a ray with the bounds
        public static bool IntersectRayAABB(Ray ray, BoundsFloat3 bounds, out float distance)
        {
            distance = 0;
            float tmin = (bounds.Min.x - ray.origin.x) / ray.direction.x;
            float tmax = (bounds.Max.x - ray.origin.x) / ray.direction.x;

            if (tmin > tmax) (tmin, tmax) = (tmax, tmin);

            float tymin = (bounds.Min.y - ray.origin.y) / ray.direction.y;
            float tymax = (bounds.Max.y - ray.origin.y) / ray.direction.y;

            if (tymin > tymax) (tymin, tymax) = (tymax, tymin);

            if ((tmin > tymax) || (tymin > tmax)) return false;

            if (tymin > tmin) tmin = tymin;
            if (tymax < tmax) tmax = tymax;

            float tzmin = (bounds.Min.z - ray.origin.z) / ray.direction.z;
            float tzmax = (bounds.Max.z - ray.origin.z) / ray.direction.z;

            if (tzmin > tzmax) (tzmin, tzmax) = (tzmax, tzmin);

            if ((tmin > tzmax) || (tzmin > tmax)) return false;

            if (tzmin > tmin) tmin = tzmin;
            if (tzmax < tmax) tmax = tzmax;

            distance = tmin;
            return true;
        }

        // Converts bounds to a Rect projected onto a specified plane (XZ by default)
        public Rect ToRect(Plane projectionPlane = Plane.XZ)
        {
            float2 min, size;

            switch (projectionPlane)
            {
                case Plane.XY:
                    min = new float2(Min.x, Min.y);
                    size = new float2(Size.x, Size.y);
                    break;
                case Plane.XZ:
                    min = new float2(Min.x, Min.z);
                    size = new float2(Size.x, Size.z);
                    break;
                case Plane.YZ:
                    min = new float2(Min.y, Min.z);
                    size = new float2(Size.y, Size.z);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new Rect(min.x, min.y, size.x, size.y);
        }

        // Static method to create bounds from a collection of points
        public static BoundsFloat3 CreateFromPoints(IEnumerable<float3> points)
        {
            float3 min = new float3(float.MaxValue);
            float3 max = new float3(float.MinValue);

            foreach (var point in points)
            {
                min = math.min(min, point);
                max = math.max(max, point);
            }

            return new BoundsFloat3((min + max) * 0.5f, max - min);
        }
    }

    public enum Plane
    {
        XY,
        XZ,
        YZ
    }
}
