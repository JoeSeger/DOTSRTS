using DOTSRTS.Utilities.DOTSGrid.Components._2D;
using DOTSRTS.Utilities.DOTSGrid.Data;
using DOTSRTS.Utilities.DOTSGrid.Systems._2D;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTSGrid.Authoring._2D
{
    public class Grid2dAuthoring : MonoBehaviour
    {
        public int ID => id;
        public GridData GridData => gridData;


        [SerializeField] private int id;
        [SerializeField] private GridData gridData;

        [ShowInInspector] public int InstanceID => gameObject.GetInstanceID();

        // Baker class for converting MonoBehaviour to ECS data
        public class Grid2dBaker : Baker<Grid2dAuthoring>
        {
            public override void Bake(Grid2dAuthoring authoring)
            {
                var gridData = authoring.gridData;
                if (gridData == null) return;

                var serializableGrid = gridData.SerializableGrids[authoring.id];
                if (serializableGrid == null) return;

                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<Grid2d>(entity);
                DependsOn(gridData);
#if UNITY_EDITOR

                // Add the SerializableGridData component
                AddComponentObject(entity, new SerializableGridData
                {
                    Value = serializableGrid,
                });
#endif


                var transform = GetComponent<Transform>(authoring);
                if (transform == null) return;

                DependsOn(transform);

                AddComponent(entity, new GridComponent
                {
                    ID = authoring.id,
                    InstanceID = authoring.InstanceID
                });
                DependsOn(gridData);
            }
        }
    }
}