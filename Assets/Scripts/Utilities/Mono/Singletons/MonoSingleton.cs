using UnityEngine;

namespace DOTSRTS.Utilities.Mono.Singletons
{
    public abstract class MonoSingleton<T> : MonoBehaviour,  IMonoSingleton<T> where T : Component
    {
        public static T Instance => IMonoSingleton<T>.Instance;
    }
}