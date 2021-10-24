using Managers;
using UnityEngine;

namespace Loots
{
    public class InventoryItemLoot : MonoBehaviour
    {
        [SerializeField] private int _Id;
        [SerializeField] private int _minimum = 1;
        [SerializeField] private int _maximum = 1;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
        
            int count = Random.Range(_minimum, _maximum);
            GameManager.Instance.Inventory.Add(_Id, count);
            Destroy(gameObject);
        }
    }
}
