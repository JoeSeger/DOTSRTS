using DOTSRTS.Events.Controls;
using DOTSRTS.Events.Editor;
using DOTSRTS.Utilities.CSharp.Controls;
using DOTSRTS.Utilities.DOTS.Attributes;
using DOTSRTS.Utilities.DOTS.Editor;
using DOTSRTS.Utilities.DOTSGrid.Components;
using DOTSRTS.Utilities.DOTSGrid.Data;
using Drawing;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTSGrid.Editor
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.EntityProxy |
                       WorldSystemFilterFlags.EntityProxyPreview)]
    [UpdateOnSceneEditor, UpdateOnSceneGUI]
    public partial class EditorActiveGridVisualSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<ActiveGrid>();
        }

        protected override void OnUpdate()
        {
            foreach (var (gridComponent, entity) in SystemAPI.Query<RefRO<GridComponent>>().WithAll<ActiveGrid>()
                         .WithEntityAccess())
            {
                var gridBounds = gridComponent.ValueRO.Bounds;
                Draw.WireBox(gridBounds.Center, gridBounds.Size, Color.red);
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.EntityProxy |
                       WorldSystemFilterFlags.EntityProxyPreview)]
    [UpdateOnSceneEditor, UpdateOnSceneGUI]
    public partial class EditorFocusGridVisualSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<FocusGrid>();
        }

        protected override void OnUpdate()
        {
            foreach (var (gridComponent, entity) in SystemAPI.Query<RefRO<GridComponent>>().WithAll<FocusGrid>()
                         .WithNone<ActiveGrid>()
                         .WithEntityAccess())
            {
                var gridBounds = gridComponent.ValueRO.Bounds;
                Draw.WireBox(gridBounds.Center, gridBounds.Size, Color.blue);
            }
        }
    }


    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.EntityProxy |
                       WorldSystemFilterFlags.EntityProxyPreview)]
    [UpdateOnSceneEditor, UpdateOnSceneGUI]
    public partial class EditorFocusGridGridSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<EditorSceneViewFocusTag>();

        }
        
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (gridComponent, entity) in SystemAPI.Query<RefRW<GridComponent>>().WithEntityAccess())
            {
                gridComponent.ValueRW.MouseOver =
                    EditorGridTools.EditorSceneViewIsMouseOverBounds(gridComponent.ValueRO.Bounds);

                var isFocusGrid = EntityManager.HasComponent<FocusGrid>(entity);
                if (gridComponent.ValueRW.MouseOver)
                {
                    if (isFocusGrid) continue;
                    ecb.AddComponent<FocusGrid>(entity);

                    EditorEventManager.EditorEventAggregator.Publish(new OnGridFocusEvent
                    {
                        GridComponent = gridComponent.ValueRW,
                    });
                }
                else
                {
                    if (isFocusGrid)
                    {
                        ecb.RemoveComponent<FocusGrid>(entity);
                    }
                }
            }

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}