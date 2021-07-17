using System;
using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEngine;

public class Ability : MonoBehaviour
{
    [Serializable]
    public class Phase
    {
        [SerializeField] private string _animationTrigger;
        [SerializeField] private float _duration;
        [SerializeField] private bool _canMove;
        [SerializeField] private bool _faceTarget;
        [SerializeField] private int _priority;
        
        public float Duration => _duration;
        public bool CanMove => _canMove;
        public bool FaceTarget => _faceTarget;
        public int Priority => _priority;
        public string AnimationTrigger => _animationTrigger; // TODO cache animation
    }
    
    [SerializeField] private Phase _castPhase; // cast: interruptible
    [SerializeField] private Phase _executePhase; // execute: not interruptible, block movement, animation
    [SerializeField] private Phase _recoveryPhase; // post execute animation, block movement, GCD
    [SerializeField] private Phase _cooldownPhase; // cooldown before ready
    
    [SerializeField] private GameObject _executePrefab;

    private List<Phase> _phases;

    private Phase _currentPhase = null;
    private Transform _transform = null;

    public bool CanStartInMovement => _castPhase.CanMove;
    public int Priority => _currentPhase?.Priority ?? -1;
    public bool CanMove => _currentPhase?.CanMove ?? true;
    public bool FaceTarget => _currentPhase?.FaceTarget ?? false;
    public bool Ready => _currentPhase == null;

    public ITargetable Target { get; private set; }

    private Coroutine _coroutine;

    private float _currentPhaseStartTime;

    private Animator _animator;
    
    // Current state progress. NaN if _currentPhase is null
    public float Progress => (Time.time - _currentPhaseStartTime) / (_currentPhase?.Duration ?? 0);
    
    public event Action<Phase> PhaseChanged;

    private void Awake()
    {
        _transform = transform;
        _phases = new List<Phase>() {_castPhase, _executePhase, _recoveryPhase, _cooldownPhase};
    }

    public bool UseAbility(ITargetable target)
    {
        if (_currentPhase != null)
            return false;
        Target = target;

        _coroutine = StartCoroutine(UseAbilityCoroutine());
        return true;
    }

    private void ExecuteAbility()
    {
        var collider1 = _transform.parent.GetComponent<Collider>();
        var gameObject = Instantiate(_executePrefab, _transform.position, _transform.rotation);
        var collider2 = gameObject.GetComponent<Collider>();
        Physics.IgnoreCollision (collider1, collider2);
    }
    
    private IEnumerator UseAbilityCoroutine(int initialPhaseIndex = 0)
    {
        for (int i = initialPhaseIndex; i < _phases.Count; i++)
        {
            _currentPhase =  _phases[i];
            _currentPhaseStartTime = Time.time;
            PhaseChanged?.Invoke(_currentPhase);

            if (_currentPhase.Duration > 0)
                yield return new WaitForSeconds(_currentPhase.Duration);
            if (_currentPhase == _executePhase && _executePrefab != null)
                ExecuteAbility();
        }

        _coroutine = null;
        _currentPhase = null;
        Target = null;
    }

    /// <summary>
    /// another ability or walking took priority.
    /// cancel this ability. return to Ready state if casting, else move to cooldown
    /// </summary>
    public void CancelAbility()
    {
        Debug.Log($"CancelAbility: {name} by {transform.parent.name}");
        if (_coroutine == null)
            return;
        if (_currentPhase == _castPhase)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
            _currentPhase = null;
            Target = null;
        }
        else if (_currentPhase != _cooldownPhase)
        {
            StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(UseAbilityCoroutine(3));
            _currentPhase = _cooldownPhase;
        }
    }
}
