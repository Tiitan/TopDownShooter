using System.ComponentModel;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private string resourceId;

        private TMP_Text _text;
        private Inventory _inventory;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        void Start()
        {
            _inventory = GameManager.Instance.Inventory;
            _inventory.PropertyChanged += InventoryOnPropertyChanged;
            _text.text = _inventory.GetItem(resourceId).ToString();
        }

        private void OnDestroy()
        {
            if (_inventory)
                _inventory.PropertyChanged -= InventoryOnPropertyChanged;
        }

        private void InventoryOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == resourceId)
                _text.text = _inventory.GetItem(resourceId).ToString();
        }
    }
}
