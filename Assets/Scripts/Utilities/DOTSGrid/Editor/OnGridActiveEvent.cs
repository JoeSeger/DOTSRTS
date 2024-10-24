using DOTSRTS.Utilities.DOTSGrid.Data;

namespace DOTSRTS.Utilities.DOTSGrid.Editor
{
    public class OnGridActiveEvent
    {
        public GridComponent GridComponent;

#if UNITY_EDITOR
        public SerializableGrid SerializableGrid;
#endif
    }
}