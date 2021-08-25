using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interface;
using UnityEngine;

namespace Characters
{
    /// <summary>
    /// Spawn new mobs at each waves.
    /// Managed by LevelManager,
    /// this component should be used on a GameObject with "Spawner" tag
    /// </summary>
    public class MultiSpawner : MonoBehaviour, ISpawner
    {
        [Serializable]
        struct SpawnItem
        {
            [SerializeField] private int _wave;
            [SerializeField] private float _delay;
            [SerializeField] private GameObject _prefab;

            public int Wave => _wave;
            public float Delay => _delay;
            public GameObject Prefab => _prefab;
        }

        private Transform _transform;
    
        [SerializeField] private List<SpawnItem> _spawnItems = new List<SpawnItem>();

        public IEnumerable<int> Waves => _spawnItems.Select(x => x.Wave);

        private void Awake()
        {
            _transform = transform;
        }

        public int Spawn(int currentWave)
        {
            var spawnItems = _spawnItems.Where(x => x.Wave == currentWave).ToList();
            foreach (SpawnItem spawnItem in spawnItems)
                StartCoroutine(SpawnRoutine(spawnItem.Delay, spawnItem.Prefab));
            return spawnItems.Count;
        }

        private IEnumerator SpawnRoutine(float delay, GameObject prefab)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);
            Instantiate(prefab, _transform.position, _transform.rotation);
        }
    
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 1);
        }
#endif //UNITY_EDITOR
    }
}
