using UnityEngine;

[RequireComponent(typeof(Ability))]
public class AbilityListener : MonoBehaviour
{
    private Ability _ability;
    private Animator _animator;
    private static readonly int ActionSpeed = Animator.StringToHash("ActionSpeed");
    
    void Awake()
    {
        _ability = GetComponent<Ability>();
        _animator = transform.parent.GetComponentInChildren<Animator>();
        if (_animator == null)
            Debug.LogWarning($"Animator parent missing animator ({name})");
        
        _ability.PhaseChanged += OnPhaseChanged;
    }

    void OnPhaseChanged(Ability.Phase currentPhase)
    {
        if (_animator && !string.IsNullOrEmpty(currentPhase.AnimationTrigger))
        {
            _animator.SetTrigger(currentPhase.AnimationTrigger);
            // all actions default duration normalized to 1 sec.
            _animator.SetFloat(ActionSpeed, 1 / currentPhase.Duration); 
        }
    }
}
