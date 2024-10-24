namespace DOTSRTS.Utilities.CSharp
{
    public abstract class SingletonBase
    {
        protected static readonly object Lock = new(); // Object to lock on for thread safety
    }
    
    public class Singleton<T> : SingletonBase  where T : class, new()
    {
       
        private static T _instance;
        
        protected Singleton() { }
        
        public static T Instance
        {
            get
            {
                // Double-checked locking pattern for thread-safe initialization
                if (_instance != null) return _instance;
                lock (Lock) // Lock to ensure thread-safe singleton initialization
                {
                    _instance ??= new T();
                }
                return _instance;
            }
        }
    }
}