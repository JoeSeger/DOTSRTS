
using DOTSRTS.Events;
using DOTSRTS.Utilities.ScriptableObjects;



namespace DOTSRTS.SceneManagement.Data
{
    
    public abstract class EventAggregatorSingletons<T> : EventAggregator where T : EventAggregatorSingletons<T>
    {
        private static volatile T _instance;
        protected static readonly object Lock = new();
        public static T Instance => ScriptableObjectTools.ScriptableObjectInstance(_instance, Lock);
        
    }
    
    [UnityEngine.CreateAssetMenu(fileName = "GameSceneManagerEventAggregator", menuName = "Events/EventAggregator/GameSceneManager", order = 0)]
    public  class GameSceneManagerEventAggregator : EventAggregatorSingletons<GameSceneManagerEventAggregator>
    {
       
    }
}