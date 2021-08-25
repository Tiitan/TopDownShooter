using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Characters
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Character))]
    public class Npc : MonoBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        private Character _target;
        private Character _character;
    
        [Tooltip("Recalculate path if new target position is more than x meters from destination (performance optimisation)")]
        [SerializeField] private float _followMaxMove = 0.5f;
        [Tooltip("Recalculate destination period in seconds (performance optimisation)")]
        [SerializeField] private float _recalculateDestinationPeriod = 0.2f;
    
        private void Awake()
        {
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

        private void OnDie()
        {
            Debug.Log($"NPC {name} die");
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
