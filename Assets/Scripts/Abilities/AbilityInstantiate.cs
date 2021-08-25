using Interface;
using UnityEngine;

namespace Abilities
{
    /// <summary>
    /// component part of Abilities prefab instantiating an object (usually a projectile).
    /// ExecuteAbility() should be called from Ability._OnExecuteAbility event
    /// </summary>
    public class AbilityInstantiate : MonoBehaviour
    {
        [SerializeField] private GameObject _executePrefab;
    
        private Transform _transform = null;
    
        private void Awake()
        {
            _transform = transform;
        }
    
        public void ExecuteAbility(ITargetable target)
        {
            var collider1 = _transform.parent.GetComponent<Collider>();
            GameObject instance = Instantiate(_executePrefab, _transform.position, _transform.rotation);
            var collider2 = instance.GetComponent<Collider>();
            if (collider1 != null && collider2 != null)
                Physics.IgnoreCollision (collider1, collider2);
        }
    }
}
