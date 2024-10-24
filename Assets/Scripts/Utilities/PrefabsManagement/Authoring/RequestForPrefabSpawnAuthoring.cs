using System;
using System.Collections.Generic;
using System.Linq;
using DOTSRTS.Utilities.PrefabsManagement.Components;
using Drawing;
using Sirenix.OdinInspector;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTSRTS.Utilities.PrefabsManagement.Authoring
{
    public static class BakingTools
    {
        public static void BakePrefab<T>(Baker<T> baker, T authoring, GameObject prefab) where T : Component
        {
            var entity = baker.GetEntity(TransformUsageFlags.Dynamic);
            var prefabCollection = PrefabCollection.Instance;
            if (prefabCollection == null) return;

            if (prefab == null) return;
            var prefabName = prefab.name;
            var requestingPrefab = prefabCollection.GetPrefab(prefabName);

            if (requestingPrefab == null)
            {
                Debug.LogError($"Failed to Bake because {prefabName} is not valid ");
                return;
            }

            baker.AddComponent(entity, new PrefabSpawnRequest
            {
                guid = requestingPrefab.hash
            });
        }
    }

    [BakeDerivedTypes]
    public class RequestForPrefabSpawnDebug : IComponentData
    {
    }

    public class RequestForPrefabSpawnAuthoring : MonoBehaviour
    {
        [SerializeField, ValueDropdown("AllPrefabs", DoubleClickToConfirm = true, IsUniqueList = true)]
        public GameObject prefab;

        public class RequestForPrefabSpawnBaker : Baker<RequestForPrefabSpawnAuthoring>
        {
            public override void Bake(RequestForPrefabSpawnAuthoring authoring)
            {
                BakingTools.BakePrefab(this, authoring, authoring.prefab);
            }
        }


        public IEnumerable<ValueDropdownItem<GameObject>> AllPrefabs => PrefabCollection.Instance.AllPrefabsDropDown;
    }


    [Serializable]
    public class BakingPrefabData
    {
        public int id;

        [OnValueChanged(nameof(OnValidate)),
         ValueDropdown("AllPrefabs", IsUniqueList = false, DoubleClickToConfirm = true)]
        public GameObject prefab;

        public bool editTransform;
        public const string EditTransformBool = "@" + nameof(editTransform) + " == true";
        [ShowIf(EditTransformBool)] public float3 position;
        [ShowIf(EditTransformBool)] public quaternion rotation;
        [ShowIf(EditTransformBool)] public float scale;
        public IEnumerable<ValueDropdownItem<GameObject>> AllPrefabs => PrefabCollection.Instance.AllPrefabsDropDown;


        // Ensure that if "None" is selected, it actually sets the selectedPrefab to null
        private void OnValidate()
        {
            if (prefab == PrefabCollection.Instance.NullPrefab)
            {
                prefab = null;
            }
        }

        [Button]
        private void ClearPrefabObject()
        {
            prefab = null;
        }
    }
}