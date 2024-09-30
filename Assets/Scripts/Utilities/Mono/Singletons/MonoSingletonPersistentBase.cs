using UnityEngine;

namespace DOTSRTS.Utilities.Mono.Singletons
{
    public abstract class MonoSingletonPersistentBase : MonoBehaviour
    {
        protected static readonly object Lock = new();
    }
}