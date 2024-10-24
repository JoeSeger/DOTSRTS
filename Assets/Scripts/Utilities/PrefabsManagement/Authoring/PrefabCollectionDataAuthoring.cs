using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;

namespace DOTSRTS.Utilities.PrefabsManagement.Authoring
{
    public class PrefabCollectionDataAuthoring : MonoBehaviour
    {
        [SerializeField] private PrefabCollection prefabCollection;


#if UNITY_EDITOR

        public void Reset()
        {
            prefabCollection = PrefabCollection.Instance;
        }
#endif
        public class PrefabCollectionDataBaker : Baker<PrefabCollectionDataAuthoring>
        {
            public override void Bake(PrefabCollectionDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<PrefabCollectionData>(entity);
                var prefabBuffer = AddBuffer<EntityPrefabBuffer>(entity);
                foreach (var prefabData in authoring.prefabCollection.PrefabData)
                {
                    var entityPrefabRef = new EntityPrefabReference(prefabData.value);

                    prefabBuffer.Add(new EntityPrefabBuffer
                    {
                        Name = prefabData.Name,
                        Guid = entityPrefabRef.AssetGUID,
                        EntityPrefabReference = entityPrefabRef
                    });
                }
            }
        }
    }
}