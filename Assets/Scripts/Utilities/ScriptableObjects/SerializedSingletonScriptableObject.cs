using Sirenix.OdinInspector;

namespace DOTSRTS.Utilities.ScriptableObjects
{
    public abstract class SerializedSingletonScriptableObject<T> : SerializedSingletonScriptableObjectBase where T : SerializedScriptableObject
    {
        private static volatile T _instance;

        public static T Instance => ScriptableObjectTools.ScriptableObjectInstance(_instance, Lock);

    }
}