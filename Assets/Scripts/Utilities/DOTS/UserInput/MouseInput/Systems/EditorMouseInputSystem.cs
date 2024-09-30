using DOTSRTS.Utilities.DOTS.Components;
using DOTSRTS.Utilities.DOTS.UserInput.MouseInput.Components;
using Unity.Entities;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTS.UserInput.MouseInput.Systems
{
    public partial class EditorMouseInputSystem : OnSceneGUISystemBase
    {
        private Entity _editorMouseInputDataEntity;

        protected override void OnCreate()
        {
            base.OnCreate();

            if (!EntityManager.Exists(_editorMouseInputDataEntity))
            {
                _editorMouseInputDataEntity = EntityManager.CreateEntity(typeof(UserMouseInput), typeof(EditorTag));
            }

            RequireForUpdate<SceneViewFocusTag>();
        }

        protected override void OnUpdate()
        {
            // Get the current mouse position
            Vector2 mousePosition = CurrentSceneEvent.mousePosition;

            // Check for left and right mouse button clicks
            bool leftClick = CurrentSceneEvent.button == 0 && CurrentSceneEvent.type == EventType.MouseDown;
            bool rightClick = CurrentSceneEvent.button == 1 && CurrentSceneEvent.type == EventType.MouseDown;

            // Update the UserMouseInput component with the new input data
            EntityManager.SetComponentData(_editorMouseInputDataEntity, new UserMouseInput
            {
                Position = mousePosition,
                LeftMouseClick = leftClick,
                RightMouseClick = rightClick
            });
        }
    }
}