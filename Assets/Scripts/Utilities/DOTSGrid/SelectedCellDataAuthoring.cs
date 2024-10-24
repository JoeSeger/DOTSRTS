using DOTSRTS.Utilities.DOTSGrid.Components;
using Unity.Entities;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTSGrid
{
    public class SelectedCellDataAuthoring : MonoBehaviour
    {
        public class SelectedCellDataBaker : Baker<SelectedCellDataAuthoring>
        {
            public override void Bake(SelectedCellDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<SelectedCellData>(entity);
            }
        }
    }
}