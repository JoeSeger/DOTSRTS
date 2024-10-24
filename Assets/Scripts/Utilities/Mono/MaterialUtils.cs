using UnityEngine;

namespace DOTSRTS.Utilities.Mono
{
    public static class MaterialUtils
    {
        /// <summary>
        /// Calculates the average color of a material's main texture.
        /// </summary>
        /// <param name="material">The material to calculate the average color for.</param>
        /// <returns>The average color, or white if the material has no texture.</returns>
        public static Color GetAverageColor(Material material)
        {
            if (material == null || !material.HasProperty("_MainTex"))
            {
                Debug.LogWarning("Material is null or does not have a _MainTex property.");
                return Color.white;
            }

            Texture2D texture = material.mainTexture as Texture2D;
            if (texture == null)
            {
                Debug.LogWarning("Material's main texture is not a Texture2D.");
                return Color.white;
            }

            // Get the pixels of the texture
            Color[] pixels = texture.GetPixels();
            if (pixels == null || pixels.Length == 0)
            {
                Debug.LogWarning("Texture has no pixel data.");
                return Color.white;
            }

            // Calculate the average color
            Color averageColor = Color.black;
            foreach (Color pixel in pixels)
            {
                averageColor += pixel;
            }
            averageColor /= pixels.Length;

            return averageColor;
        }
    }
}