using UnityEngine;

namespace DOTSRTS.Utilities.ScriptableObjects
{
    public abstract class SingletonScriptableObject<T> : SingletonScriptableObjectBase where T : ScriptableObject
    {
        private static volatile T _instance;
        public static T Instance => ScriptableObjectTools.ScriptableObjectInstance(_instance, Lock);
    }
}