using UnityEngine;

namespace Loots
{
    public class HealthLoot : MonoBehaviour
    {
        [SerializeField] private float _minimum = 19;
        [SerializeField] private float _maximum = 21;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
        
            float value = Random.Range(_minimum, _maximum);
            other.SendMessage("ApplyDamage", -value);
            Destroy(gameObject);
        }
    }
}