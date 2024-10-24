using DOTSRTS.Utilities.DOTSGrid.Authoring._2D;
using DOTSRTS.Utilities.DOTSGrid.Data;

namespace DOTSRTS.Utilities.DOTSGrid.Editor
{
    public struct OnGridFocusEvent
    {
        public GridComponent GridComponent;

#if UNITY_EDITOR
        public Grid2dAuthoring Grid2dAuthoring;
#endif
    }
}