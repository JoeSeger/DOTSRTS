using DOTSRTS.SceneManagement.Components;
using DOTSRTS.SceneManagement.Mono;
using Unity.Entities;
using UnityEngine;

namespace DOTSRTS.SceneManagement.Authoring
{
    public class OwningSceneAuthoring : MonoBehaviour
    {
        public class OwningSceneBaker : Baker<OwningSceneAuthoring>
        {
            public override void Bake(OwningSceneAuthoring authoring)
            {  
                var guid =  SubSceneManagerTools.GetSubSceneFromComponent(authoring).SceneGUID;
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new OwningScene { Guid = guid});
            }
        }
    }
}