using DOTSRTS.Utilities.DOTS.Attributes;
using DOTSRTS.Utilities.DOTS.Components;
using DOTSRTS.Utilities.DOTS.Editor;
using Unity.Entities;

namespace DOTSRTS.Utilities.DOTSGrid.Editor
{
    public class SceneViewTag : IComponentData
    {
    }

    public struct EditorSceneViewFocusTag : IComponentData
    {
    }


    [WorldSystemFilter(WorldSystemFilterFlags.All)]
    [UpdateOnSceneGUI, UpdateOnSceneEditor]
    public partial class EditorSceneViewSystem : SystemBase
    {
        private Entity _editorSceneViewEntity;

        protected override void OnCreate()
        {
            base.OnCreate();

            if (!EntityManager.Exists(_editorSceneViewEntity))
            {
                _editorSceneViewEntity = EntityManager.CreateEntity(typeof(SceneViewTag), typeof(EditorTag));
                EntityManager.SetName(_editorSceneViewEntity, "EditorSceneViewEntity");
            }

            
            RequireForUpdate<EditorTag>();
        }


        protected override void OnUpdate()
        {
            if (SystemEditorTools.IsMouseOverSceneView())
            {
                EntityManager.AddComponentData(_editorSceneViewEntity, new EditorSceneViewFocusTag());
            }
            else
            {
                EntityManager.RemoveComponent<EditorSceneViewFocusTag>(_editorSceneViewEntity);
            }
        }

        protected override void OnDestroy()
        {
            _editorSceneViewEntity = Entity.Null;
            base.OnDestroy();
        }
    }
}