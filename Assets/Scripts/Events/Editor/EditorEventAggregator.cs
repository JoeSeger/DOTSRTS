using DOTSRTS.SceneManagement.Data;

namespace DOTSRTS.Events.Editor
{
    [UnityEngine.CreateAssetMenu(fileName = "EditorEventAggregator",
        menuName = "Events/EventAggregator/EditorEventAggregator", order = 0)]
    public class EditorEventAggregator : EventAggregatorSingletons<EditorEventAggregator>
    {
    }
}