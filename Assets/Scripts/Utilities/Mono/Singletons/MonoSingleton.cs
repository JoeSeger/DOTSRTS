using Sirenix.OdinInspector;

namespace DOTSRTS.Utilities.Mono.Singletons
{
    public abstract class MonoSingleton<T> : SerializedMonoBehaviour where T : MonoSingleton<T>
    {
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();
                }

                return _instance;
            }
        }

        public virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }


        private static T _instance;
    }
}