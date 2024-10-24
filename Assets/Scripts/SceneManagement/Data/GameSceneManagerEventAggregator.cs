
using DOTSRTS.Events;
using DOTSRTS.Utilities.ScriptableObjects;



namespace DOTSRTS.SceneManagement.Data
{
    
    public abstract class EventAggregatorSingletons<T> : EventAggregator where T : EventAggregatorSingletons<T>
    {
        private static volatile T _instance;
  
        public static T Instance
        {
            get
            {
                var eventAggregatorSingletons = _instance;
                return ScriptableObjectTools.ScriptableObjectInstance(ref eventAggregatorSingletons, Lock);
            }
        }
    }
    
    [UnityEngine.CreateAssetMenu(fileName = "GameSceneManagerEventAggregator", menuName = "Events/EventAggregator/GameSceneManager", order = 0)]
    public  class GameSceneManagerEventAggregator : EventAggregatorSingletons<GameSceneManagerEventAggregator>
    {
       
    }
}