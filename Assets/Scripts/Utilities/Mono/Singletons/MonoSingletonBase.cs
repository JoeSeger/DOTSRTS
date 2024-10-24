using UnityEngine;

namespace DOTSRTS.Utilities.Mono.Singletons
{
    public abstract class MonoSingletonBase : MonoBehaviour
    {
        protected static readonly object Lock = new();
    }
}