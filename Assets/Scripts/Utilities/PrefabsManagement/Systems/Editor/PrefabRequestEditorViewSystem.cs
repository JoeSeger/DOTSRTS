using System;
using DOTSRTS.Utilities.DOTS.Attributes;
using DOTSRTS.Utilities.DOTS.Editor;
using DOTSRTS.Utilities.PrefabsManagement.Authoring;
using DOTSRTS.Utilities.PrefabsManagement.Components;
using Drawing;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using Drawing;
using Unity.Transforms;

namespace DOTSRTS.Utilities.PrefabsManagement.Systems.Editor
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor)]
    [UpdateOnSceneEditor, UpdateOnSceneGUI]
    public partial class PrefabRequestEditorViewSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var prefabCollection = PrefabCollection.Instance;
            if (prefabCollection == null) return;
            var data = prefabCollection.PrefabData;
            foreach (var (request, transformRef) in SystemAPI
                         .Query<RefRO<PrefabSpawnRequest>,RefRO<LocalTransform>>())
            {
                var prefab = prefabCollection.GetPrefab(request.ValueRO.guid);
                if (prefab == null || prefab.value == null) continue;
                var mesh = prefab.value.GetComponent<MeshFilter>();

                // Get the MeshFilter component from the prefab
                var meshFilter = prefab.value.GetComponent<MeshFilter>();
                if (meshFilter == null || meshFilter.sharedMesh == null) continue;

                // Ensure the Transform is valid before using it
                var transform = transformRef.ValueRO;

             
                using (Draw.WithMatrix(transform.ToMatrix()))
                {
                    Draw.WireMesh(mesh.sharedMesh);
                }

                
            }
        }
        
        
    }
    
    

}