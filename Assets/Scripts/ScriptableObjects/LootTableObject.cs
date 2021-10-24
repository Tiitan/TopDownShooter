using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ScriptableObjects
{
    [Serializable]
    public class LootItem
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private float _chance;
        
        public GameObject Prefab => _prefab;
        public float Chance => _chance;
    }

    /// <summary>
    /// LootTable referenced by mobs (or chest) to select a reward when defeated (or opened).
    /// </summary>
    [CreateAssetMenu(fileName = "LootTable", menuName = "Custom/LootTable", order = 1)]
    public class LootTableObject : ScriptableObject
    {
        [SerializeField] private int _minimumItemCount = 0;
        [SerializeField] private int _maximumItemCount = 1;
        [SerializeField] private bool _allowDuplicate = false;
        [SerializeField] private List<LootItem> _lootTable;

        private LootItem RollLoot(float playerChance, List<LootItem> selectedLoots)
        {
            float randomNumber = Random.Range(0, 100 - playerChance);
            float i = 0;
            foreach (var lootItem in _lootTable)
            {
                i += lootItem.Chance;
                if (i > randomNumber)
                {
                    if (_allowDuplicate || !selectedLoots.Contains(lootItem))
                        return lootItem;
                }
            }

            return null;
        }
        
        /// <summary>
        /// GetLoot called whenever a mob die to select a reward for the player.
        /// </summary>
        /// <param name="playerChance">player increased loot chance, (-100 to 100)</param>
        /// <returns>List of prefab loots that should be instantiated around the dead foe</returns>
        public IEnumerable<GameObject> GetLoot(float playerChance = 0)
        {
            playerChance = playerChance > 100 ? 100 : playerChance;
            List<LootItem> selectedLoots = new List<LootItem>();
            for (int i = 0; i < _maximumItemCount || selectedLoots.Count < _minimumItemCount; i++)
            {
                LootItem loot = RollLoot(playerChance, selectedLoots);
                if (loot != null)
                    selectedLoots.Add(loot);

                if (i > _maximumItemCount * 2)
                {
                    Debug.LogWarning($"LootTable {name} did not reach minimum loots. Safe break");
                    break;
                }
            }

            return selectedLoots.Select(x => x.Prefab);
        }
    }
}
