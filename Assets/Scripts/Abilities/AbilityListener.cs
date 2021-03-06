using UnityEngine;

namespace Abilities
{
    /// <summary>
    /// AbilityListener listen to an Ability class and handle animations and Fx updates.
    /// nothing impact the gameplay here
    /// </summary>
    [RequireComponent(typeof(Ability))]
    public class AbilityListener : MonoBehaviour
    {
        private Ability _ability;
        private Animator _animator;
        private static readonly int ActionSpeed = Animator.StringToHash("ActionSpeed");
        private static readonly int Cancel = Animator.StringToHash("Cancel");

        void Awake()
        {
            _ability = GetComponent<Ability>();
            _animator = transform.parent.GetComponentInChildren<Animator>();
            if (_animator == null)
                Debug.LogWarning($"Animator parent missing animator ({name})");
        
            _ability.PhaseChanged += OnPhaseChanged;
            _ability.Cancel += OnCancel;
        }

        void OnCancel()
        {
            foreach (Ability.Phase phase in _ability.Phases)
            {
                if (phase.AnimationTrigger != string.Empty)
                    _animator.ResetTrigger(phase.AnimationTrigger);
            }
            _animator.SetTrigger(Cancel);
        }
    
        void OnPhaseChanged(Ability.Phase currentPhase)
        {
            if (!_animator) return;

            if (!string.IsNullOrEmpty(currentPhase?.AnimationTrigger))
            {
                _animator.SetTrigger(currentPhase.AnimationTrigger);
                // all actions default duration normalized to 1 sec.
                _animator.SetFloat(ActionSpeed, 1 / currentPhase.Duration); 
            }
        }
    }
}
