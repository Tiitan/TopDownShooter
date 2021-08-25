using UnityEngine;

namespace Characters
{
    /// <summary>
    /// Character listener for animation and fx update.
    /// register Character events. no gameplay here.
    /// </summary>
    [RequireComponent(typeof(Character))]
    public class CharacterListener : MonoBehaviour
    {
        private Character _character;
        private Animator _animator;
        private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
        private static readonly int MoveAngle = Animator.StringToHash("MoveAngle");

        void Awake()
        {
            _character = GetComponent<Character>();
            _animator = GetComponentInChildren<Animator>();
            if (_animator == null)
                Debug.LogWarning($"Animator parent missing animator ({name})");
        
            _character.MoveUpdate += OnMoveUpdate;
        }

        void OnMoveUpdate(float speed, float angle)
        {
            if (_animator)
            {
                _animator.SetFloat(MoveSpeed, speed);
                _animator.SetFloat(MoveAngle, angle);
            }
        }
    }
}