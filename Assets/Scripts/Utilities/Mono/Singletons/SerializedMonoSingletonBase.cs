using Sirenix.OdinInspector;

namespace DOTSRTS.Utilities.Mono.Singletons
{
    public abstract class SerializedMonoSingletonBase : SerializedMonoBehaviour
    {
        protected static readonly object Lock = new();
    }
}