using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Abilities;
using Interface;
using JetBrains.Annotations;
using Managers;
using ScriptableObjects;
using UnityEngine;

namespace Characters
{
    /// <summary>
    /// Character class used by players and NPC.
    /// </summary>
    public class Character : MonoBehaviour, INotifyPropertyChanged, ITargetable, IDamageable, IResource
    {
        [SerializeField] private float _maxHealthPoint;
    
        [Tooltip("Default 100, Should not be changed. Reference priority, " +
                 "cancel ability casting of lower priority, " +
                 "immobilise the character when casting an ability of higher priority.")]
        [SerializeField] private float _walkPriority = 100;
    
        [SerializeField] private int _team;
    
        // TODO: temporary, create player equipment management
        [SerializeField] private AbilityObject _passiveAbilityObject;

        [SerializeField] private Animator _animator;
    
        [SerializeField] private int _energy = 5;
        [SerializeField] private int _energyRegenDelay = 2; // second per energy point
    
        private float _healthPoint;
        
        private Ability _passiveAbility;
        private Transform _transform;
        private TargetManager _targetManager;
        private float _energyRegenReferenceTime;
        private bool _isStopped;
        private int _maxEnergy;

        public Ability PassiveAbility => _passiveAbility;
        public float WalkPriority => _walkPriority;
        
        public int Energy
        {
            get => _energy;
            private set
            {
                _energy = value;
                OnPropertyChanged();
            }
        }
    
        public int MaxEnergy
        {
            get => _maxEnergy;
            private set
            {
                _maxEnergy = value;
                OnPropertyChanged();
            }
        }
    
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
        public event Action Die;

        private void Awake()
        {
            _transform = transform;
            _healthPoint = _maxHealthPoint;
            _maxEnergy = _energy;
        
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

        void UpdateAbility(out float moveAngle)
        {
            moveAngle = 0;
            if (Direction != Vector3.zero)
                _transform.rotation = Quaternion.LookRotation(Direction);

            // Cancel cast if stop moving and if movement required
            if (Direction == Vector3.zero && !_passiveAbility.CanUseImmobile && _passiveAbility.Active)
            {
                _passiveAbility.TryCancel(_walkPriority);
            }
        
            // look at target if required by ability state
            if (!_passiveAbility.Ready && _passiveAbility.FaceTarget && (MonoBehaviour)_passiveAbility.Target != null)
            {
                _transform.LookAt(_passiveAbility.Target.Position, Vector3.up);
                var lookDirection = (Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * Vector3.forward).normalized;
                moveAngle = Vector3.SignedAngle(lookDirection, Direction, Vector3.up);
            }

            //shoot
            if (_passiveAbility.Ready &&
                (Direction != Vector3.zero && _passiveAbility.CanStartInMovement ||
                 Direction == Vector3.zero && _passiveAbility.CanUseImmobile))
            {
                ITargetable target = _targetManager.GetCloserEnemy(_transform.position, _team, _passiveAbility.Range);
                if (target != null)
                {
                    _transform.LookAt(target.Position, Vector3.up);
                    _passiveAbility.UseAbility(target, this);
                }
            }
        }

        void UpdateEnergy()
        {
            if (Direction != Vector3.zero)
                _isStopped = false;
                
            // just stopped moving
            if (!_isStopped && Direction == Vector3.zero)
            {
                _isStopped = true;
                _energyRegenReferenceTime = Time.time;
            }
        
            // regen energy
            if (_isStopped && Time.time > _energyRegenReferenceTime + _energyRegenDelay)
            {
                if (_energy < _maxEnergy)
                    Energy = _energy + 1;
                _energyRegenReferenceTime = Time.time;
            }
        }
        
        public void Update()
        {
            UpdateEnergy();

            float moveAngle = 0;
            if (_passiveAbility != null)
                UpdateAbility(out moveAngle);

            float moveSpeed = Direction.sqrMagnitude;
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

        public int RegenEnergy(int value)
        {
            value = Mathf.Clamp(value, 0, _maxEnergy - _energy);
            Energy = _energy + value;
            return value;
        }

        public bool TryConsumeResource(string resourceType, int cost)
        {
            // TODO other resources
            if (_energy < cost)
                return false;
            Energy = _energy - cost;
            return true;
        }
    

    
        [UsedImplicitly] // send message from Projectile and other damage sources.
        public void ApplyDamage(float damage)
        {
            HealthPoint = Mathf.Clamp(HealthPoint - damage, 0, _maxHealthPoint);
            Debug.Log($"[{name}] Damage: {damage} ({_healthPoint}/{_maxHealthPoint})");

            if (HealthPoint == 0)
                Die?.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
