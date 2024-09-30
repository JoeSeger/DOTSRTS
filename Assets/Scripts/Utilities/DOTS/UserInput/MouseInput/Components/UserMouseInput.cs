using Unity.Entities;
using Unity.Mathematics;

namespace DOTSRTS.Utilities.DOTS.UserInput.MouseInput.Components
{
    public struct UserMouseInput : IComponentData
    {
        public float2 Position;
        public bool RightMouseClick;
        public bool LeftMouseClick;
        public bool ScrollClick;
        public float ScrollValue;
    }

    public struct OldInput : IComponentData
    {
        
    }

    public struct NewInput : IComponentData
    {
        
    }
}