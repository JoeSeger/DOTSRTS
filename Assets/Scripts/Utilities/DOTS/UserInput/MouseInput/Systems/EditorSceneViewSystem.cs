using DOTSRTS.Utilities.DOTS.Components;
using DOTSRTS.Utilities.DOTS.UserInput.MouseInput.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTS.UserInput.MouseInput.Systems
{
    public class SceneViewTag : IComponentData
    {
   
    }

    public struct SceneViewFocusTag : IComponentData
    {
    }

    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.EntityProxy |
                       WorldSystemFilterFlags.EntityProxyPreview)]
    public partial class EditorSceneViewSystem : OnSceneGUISystemBase
    {
        private Entity _editorSceneViewEntity;
    

        protected override void OnCreate()
        {
            base.OnCreate();

            if (!EntityManager.Exists(_editorSceneViewEntity))
            {
                _editorSceneViewEntity = EntityManager.CreateEntity(typeof(SceneViewTag), typeof(EditorTag));
            }
            
        }
        
        protected override void OnUpdate()
        {
            
            if (_editorSceneViewEntity == Entity.Null) return;
            
            if (CurrentSceneView == null) return;

            var isFocused = CurrentSceneView.hasFocus;

            if (isFocused)
            {
                EntityManager.AddComponentData(_editorSceneViewEntity, new SceneViewFocusTag());
            }
            else
            {
                EntityManager.RemoveComponent<SceneViewFocusTag>(_editorSceneViewEntity);
            }
        }
        
    }
}