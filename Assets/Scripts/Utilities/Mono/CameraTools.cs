using Unity.Mathematics;
using UnityEngine;

namespace DOTSRTS.Utilities.Mono
{
    public static class CameraTools
    {
        public static Ray ScreenPointToRay(this Camera camera,float2 screenPosition)
        {
            Vector2 settingScenePosition = screenPosition;
           return camera.ScreenPointToRay(settingScenePosition);
        }
    }
}