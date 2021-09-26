using System;
using Abilities;
using Enums;
using JetBrains.Annotations;
using Managers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Characters
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Character))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private float _speed;
        
        private Character _character;
        private Transform _transform;
        private NavMeshAgent _navMeshAgent;

        private Vector3 _inputDirection;
        
        void Awake()
        {
            _character = GetComponent<Character>();
            _transform = GetComponent<Transform>();
            _navMeshAgent = GetComponent<NavMeshAgent>();

            _character.Die += OnDie;
        }

        [UsedImplicitly]
        public void Move(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var direction2D = context.ReadValue<Vector2>();
                _inputDirection = new Vector3(direction2D.x, 0, direction2D.y);
            }
            else if (context.canceled)
                _inputDirection = Vector3.zero;
        }
    
        [UsedImplicitly]
        public void Action(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            // TODO: action
        }

        private void Update()
        {
            if (_inputDirection != Vector3.zero)
            {
                Ability ability = _character.PassiveAbility;
                CanMoveStatus canMove = ability?.CanMove(_character.WalkPriority) ?? CanMoveStatus.Allowed;
                if (canMove != CanMoveStatus.Forbidden)
                {
                    if (canMove == CanMoveStatus.RequireCancel)
                        ability.TryCancel(_character.WalkPriority);
                    _character.Direction = _inputDirection * _speed;
                    
                    // run this script before Character, potential override during ability update
                    _transform.rotation = Quaternion.LookRotation(_inputDirection); 
                }
                else
                    _character.Direction = Vector3.zero;
            }
            else
                _character.Direction = Vector3.zero;
            _navMeshAgent.velocity = _character.Direction;
        }

        private void OnDie()
        {
            Debug.Log($"Player {name} die");
            LevelManager.Instance.OnPlayerDie();
        }
        
        private void OnDestroy()
        {
            if (_character != null)
                _character.Die -= OnDie;
        }
    }
}
