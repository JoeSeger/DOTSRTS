using DOTSRTS.Utilities.DOTSGrid.Components;
using Unity.Entities;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTSGrid.Authoring
{
    public class GridManagerDataAuthoring : MonoBehaviour
    {
        public class GridManagerDataBaker : Baker<GridManagerDataAuthoring>
        {
            public override void Bake(GridManagerDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<GridManagerData>(entity);
            }
        }
    }
}