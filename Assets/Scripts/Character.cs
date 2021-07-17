using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Interface;
using JetBrains.Annotations;
using Managers;
using ScriptableObjects;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour, INotifyPropertyChanged, ITargetable, IDamageable
{
    [SerializeField] private float _speed;
    [SerializeField] private float _animationSpeedMultiplier;
    
    [SerializeField] private float _maxHealthPoint;
    
    [Tooltip("Default 100, Should not be changed. Reference priority, " +
             "cancel ability casting of lower priority, " +
             "immobilise the character when casting an ability of higher priority.")]
    [SerializeField] private float _walkPriority = 100;
    
    [SerializeField] private int _team;
    
    // TODO: temporary, create player equipment management
    [SerializeField] private AbilityObject _passiveAbilityObject;

    [SerializeField] private Animator _animator;
    
    private float _healthPoint;
    private CharacterController _characterController;
    private Ability _passiveAbility;
    private Transform _transform;
    private TargetManager _targetManager;


    
    public float HealthPoint
    {
        get => _healthPoint;
        private set
        {
            if (value.Equals(_healthPoint)) return;
            _healthPoint = value;
            OnPropertyChanged();
        }
    }
    
    public float MaxHealthPoint
    {
        get => _maxHealthPoint;
        private set
        {
            if (value.Equals(_maxHealthPoint)) return;
            _maxHealthPoint = value;
            OnPropertyChanged();
        }
    }

    public Vector3 Direction { get; set; }

    public int Team => _team;
    public Vector3 Position => _transform.position;
    
    public event Action<float, float> MoveUpdate;

    private void Awake()
    {
        _transform = transform;
        _characterController = GetComponent<CharacterController>();
        _healthPoint = _maxHealthPoint;

        // TODO temporary, create player equipment management
        if (_passiveAbilityObject != null)
            _passiveAbility = Instantiate(_passiveAbilityObject.AbilityPrefab, _transform).GetComponent<Ability>();
    }

    private void Start()
    {
        _targetManager = LevelManager.Instance.TargetManager;
        LevelManager.Instance.TargetManager.Register(this);
        LevelManager.Instance.OverlayGuiManager.Register(this);
    }
    float _prevMoveSpeed = 0, _prevMoveAngle = 0;
    
    public void Update()
    {
        bool hasPassiveAbility = _passiveAbility != null;

        float moveSpeed = 0, moveAngle = 0;
        
        // Move
        if (Direction != Vector3.zero && 
            (!hasPassiveAbility || _passiveAbility.CanMove || _passiveAbility.Priority < _walkPriority))
        {
            if (hasPassiveAbility && !_passiveAbility.CanMove)
                _passiveAbility.CancelAbility();
            var motion = Direction * _speed;
            _characterController.Move(motion * Time.deltaTime);
            _transform.rotation = Quaternion.LookRotation(Direction);

            moveSpeed = motion.sqrMagnitude * _animationSpeedMultiplier;
        }

        // look at target if required by ability state
        if (hasPassiveAbility && !_passiveAbility.Ready && _passiveAbility.FaceTarget && (MonoBehaviour)_passiveAbility.Target != null)
        {
            _transform.LookAt(_passiveAbility.Target.Position, Vector3.up);
            var lookDirection = (Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * Vector3.forward).normalized;
            moveAngle = Vector3.SignedAngle(lookDirection, Direction, Vector3.up);
        }

        //shoot
        if (hasPassiveAbility && _passiveAbility.Ready &&
            (Direction == Vector3.zero || _passiveAbility.CanStartInMovement))
        {
            ITargetable target = _targetManager.GetCloserEnemy(_transform.position, _team);
            if (target != null)
            {
                _transform.LookAt(target.Position, Vector3.up);
                _passiveAbility.UseAbility(target);
            }
        }

        // Update animation in CharacterListener if required
        if (Math.Abs(_prevMoveSpeed - moveSpeed) > 0.05f || 
            Math.Abs(_prevMoveAngle - moveAngle) > 0.05f)
        {
            MoveUpdate?.Invoke(moveSpeed, moveAngle);
            _prevMoveSpeed = moveSpeed;
            _prevMoveAngle = moveAngle;
        }
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance)
        {
            LevelManager.Instance.TargetManager.UnRegister(this);
            LevelManager.Instance.OverlayGuiManager.UnRegister(this);
        }
    }
    
    public void Die()
    {
        Debug.Log($"{name} die");
        Destroy(gameObject);
    }
    
    public void ApplyDamage(float damage)
    {

        if (HealthPoint > damage)
        {
            HealthPoint -= damage;
            Debug.Log($"Damage on {name}: {damage} ({_healthPoint}/{_maxHealthPoint})");

        }
        else
        {
            HealthPoint = 0;
            Debug.Log($"Damage on {name}: {damage} ({_healthPoint}/{_maxHealthPoint})");
            Die();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
