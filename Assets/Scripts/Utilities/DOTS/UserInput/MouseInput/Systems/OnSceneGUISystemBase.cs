using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTS.UserInput.MouseInput.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.EntityProxy |
                       WorldSystemFilterFlags.EntityProxyPreview)]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class OnSceneGUISystemBase : SystemBase
    {
        protected Event CurrentSceneEvent;
        protected SceneView CurrentSceneView;
        protected SceneView LastActiveSceneView;


        protected override void OnCreate()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
           return;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            CurrentSceneView = sceneView;
            CurrentSceneEvent = Event.current;
            LastActiveSceneView = SceneView.lastActiveSceneView;

            if (CheckedStateRef.ShouldRunSystem())
            {
                Update();
            }
        }

        protected override void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            base.OnDestroy();
        }
    }
}