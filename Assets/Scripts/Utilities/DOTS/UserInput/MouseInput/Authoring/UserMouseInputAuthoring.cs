using DOTSRTS.Utilities.DOTS.UserInput.MouseInput.Components;
using Unity.Entities;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTS.UserInput.MouseInput.Authoring
{
    public class UserMouseInputAuthoring : MonoBehaviour
    {
        [SerializeField] private bool oldInput;
        public class UserMouseInputBaker : Baker<UserMouseInputAuthoring>
        {
            public override void Bake(UserMouseInputAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<UserMouseInput>(entity);
                if (authoring.oldInput)
                {
                    AddComponent<OldInput>(entity);
                }
                else
                {
                    AddComponent<NewInput>(entity);
                }
            }
        }
    }
}