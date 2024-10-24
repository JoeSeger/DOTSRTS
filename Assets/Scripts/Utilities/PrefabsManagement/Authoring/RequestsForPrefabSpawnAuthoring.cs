using System;
using System.Collections.Generic;
using System.Linq;
using DOTSRTS.Utilities.PrefabsManagement.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DOTSRTS.Utilities.PrefabsManagement.Authoring
{
    public class RequestsForPrefabSpawnAuthoring : MonoBehaviour
    {
        public List<BakingPrefabData> prefabsList;
        [SerializeField] private PrefabCollection prefabCollection;

#if UNITY_EDITOR
        private void Reset()
        {
            prefabCollection = PrefabCollection.Instance;
        }
#endif

        public class RequestForPrefabSpawnBaker : Baker<RequestsForPrefabSpawnAuthoring>
        {
            public override void Bake(RequestsForPrefabSpawnAuthoring authoring)
            {
                var prefabs = authoring.prefabsList;

                // Validate the prefab list
                if (prefabs == null || !prefabs.Any())
                {
                    Debug.LogWarning("Prefab list is null or empty. Aborting baking process.");
                    return;
                }

                var prefabCollection = authoring.prefabCollection;
                // Validate the PrefabCollection
           
                for (int i = 0; i < prefabs.Count; i++)
                {
                    var prefabData = prefabs[i];
                    if (prefabData.prefab == null)
                    {
                        Debug.LogWarning($"Prefab at index {i} is null. Skipping this prefab.");
                        continue;
                    }

                    var prefabName = prefabData.prefab.name;
                    var spawningEntity = CreateAdditionalEntity(TransformUsageFlags.Dynamic, false, prefabName);
                    var requestingPrefab = prefabCollection.GetPrefab(prefabName);
                    // Add the PrefabSpawnRequest component
                    AddComponent(spawningEntity, new PrefabSpawnRequest
                    {
                        guid = requestingPrefab.hash,
                        position = prefabData.position,
                        rotation = prefabData.rotation,
                        Scale = prefabData.scale
                        
                    });
                    
                }
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    [UpdateInGroup(typeof(BakingSystemGroup),OrderLast = true)]
    [UpdateAfter(typeof(TransformBakingSystemGroup))]
    public partial class RequestLoadTransform : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<PrefabSpawnRequest>();
        }

        protected override void OnUpdate()
        {
            foreach (var (requestRef,transformRef) in SystemAPI.Query<RefRO<PrefabSpawnRequest>, RefRW<LocalTransform>>())
            {
                var request = requestRef.ValueRO;
                
                Debug.Log($"scele {request.Scale}");
                transformRef.ValueRW = LocalTransform.FromPositionRotationScale(request.position,request.rotation,request.Scale);
            }
        }
    }
}