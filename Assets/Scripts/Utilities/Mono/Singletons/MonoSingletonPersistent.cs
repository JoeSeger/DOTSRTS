using UnityEngine;

namespace DOTSRTS.Utilities.Mono.Singletons
{
    public abstract class MonoSingletonPersistent<T> : MonoSingletonPersistentBase where T : Component
    {
        private static T _instance;

        public static T Instance 
        {
            get
            {
                if (_instance != null) return _instance;
                lock (Lock)
                {
                    if (_instance == null)
                    {
                        InitializeSingleton();
                    }
                }
                return _instance;
            }
        }

        protected virtual void OnRefresh()
        {
            // Default implementation (can be overridden in derived classes)
        }

        public virtual void Awake()
        {
            InitializeInstance();
        }

        private static void InitializeSingleton()
        {
            _instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);
            if (_instance == null)
            {
                var singletonObject = new GameObject(typeof(T).Name);
                _instance = singletonObject.AddComponent<T>();
                if (Application.isPlaying)
                {
                    DontDestroyOnLoad(singletonObject);
                }
            }
            (_instance as MonoSingletonPersistent<T>)?.OnRefresh();
        }

        private void InitializeInstance()
        {
            lock (Lock)
            {
                if (_instance == null)
                {
                    _instance = this as T;
                    if (Application.isPlaying)
                    {
                        DontDestroyOnLoad(gameObject);
                    }

                    OnRefresh();
                }
                else if (_instance != this)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}