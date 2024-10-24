using Sirenix.OdinInspector;

namespace DOTSRTS.Utilities.ScriptableObjects
{
    public abstract class SerializedSingletonScriptableObject<T> : SerializedSingletonScriptableObjectBase where T : SerializedScriptableObject
    {
        private static volatile T _instance;

        public static T Instance
        {
            get
            {
                var serializedScriptableObject = _instance;
                return ScriptableObjectTools.ScriptableObjectInstance(ref serializedScriptableObject, Lock);
            }
        }
    }
}