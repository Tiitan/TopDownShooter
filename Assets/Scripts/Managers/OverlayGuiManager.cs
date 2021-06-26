using System.Collections.Generic;
using Interface;
using UnityEngine;

namespace Managers
{
    public class OverlayGuiManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _healthBarPrefab;

        [SerializeField]
        private Transform _canvasTransform;
        
        private readonly Dictionary<IDamageable,List<GameObject>> _overlayElements = new Dictionary<IDamageable, List<GameObject>>();
        
        public void Register(IDamageable damageable)
        {
            GameObject healthBar = Instantiate(_healthBarPrefab, _canvasTransform);
            healthBar.BroadcastMessage("Init", damageable);
            if (!_overlayElements.ContainsKey(damageable))
                _overlayElements[damageable] = new List<GameObject>();
            _overlayElements[damageable].Add(healthBar);
        }
        
        public void UnRegister(IDamageable damageable)
        {
            foreach (var overlayElement in _overlayElements[damageable])
                Destroy(overlayElement);
            _overlayElements.Remove(damageable);
        }
    }
}
