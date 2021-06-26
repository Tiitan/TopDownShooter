using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Framework
{
    public class ColliderTriggerEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent<GameObject>  _onTriggerEnter;
        [SerializeField] private UnityEvent<GameObject>  _onTriggerExit;
    
        void OnTriggerEnter(Collider other)
        {
            _onTriggerEnter.Invoke(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            _onTriggerExit.Invoke(other.gameObject);
        }
    }
}
