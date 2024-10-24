using DOTSRTS.Utilities.PrefabsManagement.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Scenes;
using Unity.Transforms;
using UnityEngine;

namespace DOTSRTS.Utilities.PrefabsManagement.Systems
{
    public partial struct SpawnEntityPrefabSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PrefabSpawnRequest>();
            state.RequireForUpdate<PrefabLoadResult>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);


            foreach (var (request, transformRef, entity) in SystemAPI
                         .Query<RefRO<PrefabLoadResult>, RefRO<LocalTransform>>().WithAll<PrefabSpawnRequest>()
                         .WithEntityAccess())
            {
                
                var transform = transformRef.ValueRO;
                var spawnedPrefabEntity = ecb.Instantiate(request.ValueRO.PrefabRoot);
                ecb.SetComponent(spawnedPrefabEntity, transform);
                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}