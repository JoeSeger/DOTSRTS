using System;
using UnityEngine;

namespace DOTSRTS.Utilities.Mono.Singletons
{
    public abstract class MonoSingletonPersistent<T> : MonoSingleton<T>,IPersistentGameObject where T : Component
    {
      
    }
}