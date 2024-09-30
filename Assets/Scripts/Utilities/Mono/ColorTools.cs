using Unity.Mathematics;
using UnityEngine;

namespace DOTSRTS.Utilities.Mono
{
    public static class ColorTools
    {
        public static Color ToColor(this float4 float4) =>
            new(float4.x, float4.y, float4.z, float4.w);

        public static float4 ToFloat4(this Color color) => 
            new(color.r, color.g, color.b, color.a);
    }
}