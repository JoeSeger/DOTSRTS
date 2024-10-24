using DOTSRTS.Utilities.PrefabsManagement.Authoring;
using DOTSRTS.Utilities.PrefabsManagement.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

namespace DOTSRTS.Utilities.PrefabsManagement.Systems
{
    public partial struct LoadPrefabSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PrefabCollectionData>();
            state.RequireForUpdate<EntityPrefabBuffer>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var prefabCollectionDataEntity = SystemAPI.GetSingletonEntity<PrefabCollectionData>();
            var prefabCollectionData = SystemAPI.GetBuffer<EntityPrefabBuffer>(prefabCollectionDataEntity);

            // Adding the RequestEntityPrefabLoaded component will request the prefab to be loaded.
            // It will load the entity scene file corresponding to the prefab and add a PrefabLoadResult
            // component to the entity. The PrefabLoadResult component contains the entity you can use to
            // instantiate the prefab (see the PrefabReferenceSpawnerSystem system).

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (requestEntityPrefabLoaded, entity) in SystemAPI.Query<RefRO<PrefabSpawnRequest>>()
                         .WithEntityAccess())
            {
                foreach (var prefab in prefabCollectionData)
                {
                    if (requestEntityPrefabLoaded.ValueRO.guid != prefab.Guid) continue;

                    var request = new RequestEntityPrefabLoaded
                    {
                        Prefab = prefab.EntityPrefabReference
                    };
                    ecb.AddComponent(entity, request);
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}