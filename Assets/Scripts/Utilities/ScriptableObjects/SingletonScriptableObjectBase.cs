using Sirenix.OdinInspector;
using UnityEngine;

namespace DOTSRTS.Utilities.ScriptableObjects
{
    public abstract class SingletonScriptableObjectBase : ScriptableObject
    {
        protected static readonly object Lock = new();
    }
    public abstract class SerializedSingletonScriptableObjectBase : SerializedScriptableObject
    {
        protected static readonly object Lock = new();
    }
}