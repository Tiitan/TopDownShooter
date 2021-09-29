using Managers;
using TMPro;
using UnityEngine;

namespace Gui
{
    /// <summary>
    /// Component to add on a TMP_Text (TextMeshPro text).
    /// lookup GameManager's Inventory and update the text to display the current value.
    /// Register to PropertyChanged events for dynamic display.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class ResourceDisplay : MonoBehaviour
    {
        [SerializeField] private int resourceId;

        private TMP_Text _text;
        private Inventory _inventory;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        void Start()
        {
            _inventory = GameManager.Instance.Inventory;
            _inventory.InventoryChanged += OnInventoryChanged;
            _text.text = _inventory.GetItem(resourceId).ToString();
        }

        private void OnDestroy()
        {
            if (_inventory)
                _inventory.InventoryChanged -= OnInventoryChanged;
        }

        private void OnInventoryChanged(object sender, (int, int) item)
        {
            (int itemId, int itemCount) = item;
            if (itemId == resourceId)
                _text.text = itemCount.ToString();
        }
    }
}
