using System;
using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Npc : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Character _target;
    
    [Tooltip("Recalculate path if new target position is more than x meters from destination (performance optimisation)")]
    [SerializeField] private float _followMaxMove = 0.5f;
    [Tooltip("Recalculate destination period in seconds (performance optimisation)")]
    [SerializeField] private float _recalculateDestinationPeriod = 0.2f;
    
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    private void Start()
    {
        LevelManager.Instance.RegisterMob(gameObject);
        _target = LevelManager.Instance.Player.GetComponent<Character>();
        StartCoroutine(FollowTarget());
    }

    private IEnumerator FollowTarget()
    {
        while (true)
        {
            if (Vector3.Magnitude( _navMeshAgent.destination - _target.transform.position) > _followMaxMove)
            {
                _navMeshAgent.SetDestination(_target.transform.position);
            }

            yield return new WaitForSeconds(_recalculateDestinationPeriod);
        }
    }

    // TODO: register character death instead of relying on destroy
    private void OnDestroy()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.UnRegisterMob(gameObject);
    }
}
