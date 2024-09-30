using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;

namespace DOTSRTS.Utilities.Mono
{
    [InitializeOnLoad]
    public static class CoroutineRunner
    {
        private static EditorCoroutine coroutine;

        public static void StartCoroutine(IEnumerator routine)
        {
            if (coroutine != null)
            {
                EditorCoroutineUtility.StopCoroutine(coroutine);
            }
            coroutine = EditorCoroutineUtility.StartCoroutine(routine, null);
        }
    }
}