using System;
using System.Collections.Generic;
using System.Linq;
using DOTSRTS.Utilities.Mono.GameObjects;
using DOTSRTS.Utilities.ScriptableObjects;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using Hash128 = Unity.Entities.Hash128;


namespace DOTSRTS.Utilities.PrefabsManagement
{
    [Serializable]
    public class SerializedPrefab
    {
#if UNITY_EDITOR
        [OnValueChanged(nameof(SetHash))]
#endif
        public GameObject value;

        [ShowInInspector] public Hash128 hash;
        public string Name => GetPrefabName();


#if UNITY_EDITOR
        private void SetHash()
        {
            hash = value.GetAssetGUID();
        }
#endif
        private string GetPrefabName()
        {
            return value == null ? null : value.name;
        }
    }

    [CreateAssetMenu(fileName = "PrefabCollection", menuName = "Prefabs/Collection", order = 0)]
    public class PrefabCollection : SingletonScriptableObject<PrefabCollection>
    {
        public List<SerializedPrefab> PrefabData => prefabData;
        [SerializeField] private List<SerializedPrefab> prefabData;


        public GameObject NullPrefab;

        public SerializedPrefab GetPrefab(Hash128 hash128) =>
            prefabData.FirstOrDefault(prefab => hash128 == prefab.hash);

        public SerializedPrefab GetPrefab(string prefabName) =>
            prefabData.FirstOrDefault(prefab => prefabName == prefab.value.name);

        public List<GameObject> AllPrefabs()
        {
            var list = new List<GameObject>();
            // Add the rest of the prefabs from PrefabData
            list.AddRange(PrefabData.Select(sPrefabs => sPrefabs.value));

            return list;
        }

        public IEnumerable<ValueDropdownItem<GameObject>> AllPrefabsDropDown => GetAllPrefabsDropdown();
        public ValueDropdownItem<GameObject> NullDropdownItem => new("None", NullPrefab);

        private IEnumerable<ValueDropdownItem<GameObject>> GetAllPrefabsDropdown()
        {
            var list = new List<ValueDropdownItem<GameObject>>
            {
                // Add a null option at the beginning
                NullDropdownItem
            };

            // Add the rest of the prefabs from PrefabData
            list.AddRange(PrefabData.Select(sPrefab => new ValueDropdownItem<GameObject>(sPrefab.Name, sPrefab.value)));

            return list;
        }
    }
}