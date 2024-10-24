using DOTSRTS.Utilities.DOTS.Attributes;
using UnityEditor;

namespace DOTSRTS.Utilities.DOTS.Editor
{
    [InitializeOnLoad]
    public static class OnSceneEditorSystemBase
    {
        
        static OnSceneEditorSystemBase()
        {
            EditorApplication.update += UpdateAllSceneGUISystems;
            EditorApplication.quitting += Destroy;
        }

        private static void UpdateAllSceneGUISystems()
        {
            SystemEditorTools.ForceUpdateSystemsWithAttribute<UpdateOnSceneEditorAttribute>();
        }

        private static void Destroy()
        {
            EditorApplication.update -= UpdateAllSceneGUISystems;
            EditorApplication.quitting -= Destroy;
        }

    
    }
}