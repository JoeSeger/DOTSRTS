using DOTSRTS.Utilities.CSharp.Controls;
using Unity.Mathematics;

namespace DOTSRTS.Events.Controls
{
    public struct OnMouseEventChanged
    {
        public MouseEventType MouseEventType;
        public float2 CurrentMousePosition;
        public float2 ScreenPosition;
        public float2 LastPosition;
        public bool RightButtonPressed;
        public bool LeftButtonPressed;
    }
}