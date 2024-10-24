using DOTSRTS.Events.Controls;
using DOTSRTS.Events.Editor;
using DOTSRTS.Utilities.DOTS.Attributes;
using DOTSRTS.Utilities.DOTS.Editor;
using DOTSRTS.Utilities.DOTSGrid.Components;
using DOTSRTS.Utilities.DOTSGrid.Components._2D;
using DOTSRTS.Utilities.DOTSGrid.Data;
using DOTSRTS.Utilities.Mono.GameObjects;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;


namespace DOTSRTS.Utilities.DOTSGrid.Editor
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.EntityProxy |
                       WorldSystemFilterFlags.EntityProxyPreview)]
    [UpdateOnSceneEditor, UpdateOnSceneGUI]
    public partial class EditorActiveGridGridSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<EditorSceneViewFocusTag>();
        }
        
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (gridComponent, serializableGridData,entity) in SystemAPI.Query<RefRW<GridComponent>,SerializableGridData>().WithEntityAccess())
            {
                var hasActiveGrid = EntityManager.HasComponent<ActiveGrid>(entity);
                if (EntityManager.HasComponent<FocusGrid>(entity))
                {
                    if (!EditorMouseEventManager.LeftButtonPressed) continue;
                    
                    EditorEventManager.EditorEventAggregator.Publish(new OnGridActiveEvent
                    {
                        GridComponent = gridComponent.ValueRW,
                        SerializableGrid = serializableGridData.Value
                    });
                    
                    GameObjectTools.FocusOnGameObject(EditorGridTools.GetGridObjectByInstance(gridComponent.ValueRW.InstanceID).gameObject);
                    ecb.AddComponent<ActiveGrid>(entity);
                }
                else
                {
                    if (hasActiveGrid)
                    {
                        ecb.RemoveComponent<ActiveGrid>(entity);
                    }
                }
            }

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
        
    }
}