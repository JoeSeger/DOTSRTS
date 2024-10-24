using System;
using UnityEditor;

namespace DOTSRTS.Utilities.DOTS.Editor
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class UpdateOnSceneGUIAttribute : Attribute
    {
    }

    [InitializeOnLoad]
    public static class OnSceneGUISystemBase
    {
        static OnSceneGUISystemBase()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            EditorApplication.quitting += Destroy;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            SystemEditorTools.ForceUpdateSystemsWithAttribute<UpdateOnSceneGUIAttribute>();
        }

        private static void Destroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            EditorApplication.quitting -= Destroy;
        }
    }
}