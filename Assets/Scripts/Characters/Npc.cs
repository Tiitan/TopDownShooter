using System;
using System.Collections;
using Managers;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Characters
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Character))]
    public class Npc : MonoBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        private Character _target;
        private Character _character;
        private Transform _transform;
    
        [Tooltip("Recalculate path if new target position is more than x meters from destination (performance optimisation)")]
        [SerializeField] private float _followMaxMove = 0.5f;
        [Tooltip("Recalculate destination period in seconds (performance optimisation)")]
        [SerializeField] private float _recalculateDestinationPeriod = 0.2f;
        [Tooltip("LootTable define player reward, selected from loot table object library")]
        [SerializeField] private LootTableObject _lootTable;
        [Tooltip("LootTable random position distance, increase for large mobs")]
        [SerializeField] private float _lootRadius = 1;
        
        private void Awake()
        {
            _transform = transform;
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _character = GetComponent<Character>();
            _character.Die += OnDie;
        }
    
        private void Start()
        {
            LevelManager.Instance.RegisterMob(gameObject);
            _target = LevelManager.Instance.Player.GetComponent<Character>();
            StartCoroutine(FollowTarget());
        }

        private IEnumerator FollowTarget()
        {
            while (_target != null)
            {
                if (Vector3.Magnitude( _navMeshAgent.destination - _target.transform.position) > _followMaxMove)
                {
                    _navMeshAgent.SetDestination(_target.transform.position);
                }

                yield return new WaitForSeconds(_recalculateDestinationPeriod);
            }
            // Player dead here. TODO: listen to player respawn event
        }

        private void Update()
        {
            _character.Direction = !_navMeshAgent.isStopped ? _navMeshAgent.velocity : Vector3.zero;
        }

        private void InstantiateLoots()
        {
            if (_lootTable == null) return;
            
            var loots = _lootTable.GetLoot(0);
            foreach (var loot in loots)
            {
                // randomly offset loot position
                Vector2 randomDisplacement = Random.insideUnitCircle * _lootRadius;
                Vector3 position = _transform.position;
                position.x += randomDisplacement.x;
                position.z += randomDisplacement.y;

                Instantiate(loot, position, _transform.rotation);
            }
        }
        
        private void OnDie()
        {
            Debug.Log($"NPC {name} die");

            InstantiateLoots();
            
            if (LevelManager.Instance != null)
                LevelManager.Instance.UnRegisterMob(gameObject);

            Destroy(gameObject);
        }
        
        private void OnDestroy()
        {
            if (_character != null)
                _character.Die -= OnDie;
        }
    }
}
