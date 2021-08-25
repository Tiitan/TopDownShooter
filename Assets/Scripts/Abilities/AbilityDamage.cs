using Interface;
using UnityEngine;

namespace Abilities
{
    /// <summary>
    /// component part of Abilities prefab instantiating an object (usually a projectile).
    /// ExecuteAbility() should be called from Ability._OnExecuteAbility event
    /// </summary>
    public class AbilityDamage : MonoBehaviour
    {
        [SerializeField] private float _damage;

        public void ExecuteAbility(ITargetable target)
        {
            target.ApplyDamage(_damage);
        }
    }
}
