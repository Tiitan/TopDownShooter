using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Custom/Ability", order = 1)]
    public class AbilityObject : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private Texture2D _icon;
        [SerializeField] private string _description;
        [SerializeField] private GameObject _abilityPrefab;
        
        public string Name => _name;
        public Texture2D Icon => _icon;
        public string Description => _description;
        public GameObject AbilityPrefab => _abilityPrefab;
    }
}