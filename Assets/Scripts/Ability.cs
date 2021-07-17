using System;
using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEngine;

public class Ability : MonoBehaviour
{
    [SerializeField] private bool _canUseImmobile;
    [SerializeField] private int _resourceCost = 1;
    [SerializeField] private string _resourceType; // TODO: scriptableObject
    
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
    private float _currentPhaseStartTime;
    private Transform _transform = null;
    private IResource _resource;
    
    public bool CanStartInMovement => _castPhase.CanMove;
    public int Priority => _currentPhase?.Priority ?? -1;
    public bool CanMove => _currentPhase?.CanMove ?? true;
    public bool FaceTarget => _currentPhase?.FaceTarget ?? false;
    public bool Ready => _currentPhase == null;
    public bool OnCooldown => _currentPhase == _cooldownPhase;
    public bool Active => !Ready && !OnCooldown;
    public List<Phase> Phases => _phases;
    public bool CanUseImmobile => _canUseImmobile;

    public ITargetable Target { get; private set; }
    
    private Coroutine _coroutine;

    private Animator _animator;
    
    // Current state progress. NaN if _currentPhase is null
    public float Progress => (Time.time - _currentPhaseStartTime) / (_currentPhase?.Duration ?? 0);
    
    public event Action<Phase> PhaseChanged;
    public event Action Cancel;

    private void Awake()
    {
        _transform = transform;
        _phases = new List<Phase>() {_castPhase, _executePhase, _recoveryPhase, _cooldownPhase};
    }

    public bool UseAbility(ITargetable target, IResource resource)
    {
        if (_currentPhase != null)
            return false;
        Target = target;
        _resource = resource;
        
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
    
    private void SetPhase(Phase newPhase)
    {
        _currentPhase = newPhase;
        _currentPhaseStartTime = Time.time;
        PhaseChanged?.Invoke(_currentPhase);
    }
    
    private IEnumerator UseAbilityCoroutine(int initialPhaseIndex = 0)
    {
        for (int i = initialPhaseIndex; i < _phases.Count; i++)
        {
            SetPhase(_phases[i]);
            
            // Consume resource on execute phase begin
            if (_currentPhase == _executePhase &&
                !_resource.TryConsumeResource(_resourceType, _resourceCost))
            {
                Debug.Log($"missing resource: {name} by {transform.parent.name}, {_resourceType}");
                Cancel?.Invoke();
                break;
            }

            // Yield phase duration
            if (_currentPhase.Duration > 0)
                yield return new WaitForSeconds(_currentPhase.Duration);
            
            // Apply ability on execute phase end
            if (_currentPhase == _executePhase && _executePrefab != null)
                ExecuteAbility();
        }

        _coroutine = null;
        SetPhase(null);
        Target = null;
    }

    /// <summary>
    /// another ability or walking took priority.
    /// cancel this ability. return to Ready state if casting, else move to cooldown
    /// </summary>
    public bool TryCancel(float priority)
    {
        if (_coroutine == null || priority < _currentPhase.Priority)
            return false;
        Debug.Log($"Cancel: {name} by {transform.parent.name}, TryCancel");

        if (_currentPhase == _castPhase)
        {
            Cancel?.Invoke();
            StopCoroutine(_coroutine);
            _coroutine = null;
            SetPhase(null);
            Target = null;
        }
        else if (_currentPhase != _cooldownPhase)
        {
            Cancel?.Invoke();
            StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(UseAbilityCoroutine(3));
        }

        return true;
    }
}
