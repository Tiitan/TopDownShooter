using Interface;
using JetBrains.Annotations;
using UnityEngine;

namespace Gui
{
    public class TrackTransform : MonoBehaviour
    {
        
        [SerializeField] private Vector2 _offset;
        private Transform _transform;
        private IDamageable _damageable;
        private Camera _camera;
    
        void Start()
        {
            _transform = transform;
            _camera = Camera.main;
        }

        [UsedImplicitly]
        public void Init(IDamageable damageable)
        {
            _damageable = damageable;
        }
        
        void Update()
        {
            Vector3 position = _camera.WorldToScreenPoint (_damageable.Position);
            position.x += _offset.x;
            position.y += _offset.y;
            _transform.position = position;
        }
    }
}
