using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DOTSRTS.Utilities.Mono.Singletons
{
    public interface ILockAble : IDisposable
    {
        static object Lock => new();
    }

    public interface IGameObjectComponent : IDisposable
    {
        Transform transform { get; }
        GameObject gameObject { get; }
        T GetComponent<T>() => gameObject.GetComponent<T>();
    }

    public interface IPersistentGameObject : IGameObjectComponent
    {
    }

    public interface IMonoSingleton<T> : IGameObjectComponent, ILockAble where T : Component
    {
        public static T Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                lock (Lock)
                {
                    // Second check within the lock to ensure only one thread can create the instance
                    if (_instance == null)
                    {
                        _instance = Object.FindFirstObjectByType<T>();
                        if (_instance != null) return _instance;
                        var type = typeof(T);
                        var newObj = new GameObject(type.Name);
                        _instance = newObj.AddComponent<T>();
                    }

                    return _instance;
                }
            }
        }

        void IDisposable.Dispose()
        {
            _instance = null;
            Dispose();
        }

        private static T _instance;
    }
}