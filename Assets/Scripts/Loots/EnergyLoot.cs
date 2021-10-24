using UnityEngine;

namespace Loots
{
    public class EnergyLoot : MonoBehaviour
    {
        [SerializeField] private int _value = 3;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
        
            other.SendMessage("RegenEnergy", _value);
            Destroy(gameObject);
        }
    }
}