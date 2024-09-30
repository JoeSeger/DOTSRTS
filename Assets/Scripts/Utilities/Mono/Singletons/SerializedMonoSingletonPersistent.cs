using Sirenix.OdinInspector;
using UnityEngine;

namespace DOTSRTS.Utilities.Mono.Singletons
{
    public abstract class SerializedMonoSingletonPersistent<T> : SerializedMonoBehaviour where T : Component
    {
        public static T Instance 
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = FindFirstObjectByType<T>();
                if (_instance != null)
                {
                    (_instance as SerializedMonoSingletonPersistent<T>)?.OnRefresh();
                }
                return _instance;
            }
        }
        
        public virtual void OnRefresh()
        {
            // Default implementation (can be overridden in derived classes)
        }

        public virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private static T _instance;
    }
}