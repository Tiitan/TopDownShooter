using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gui
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleTab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _mouseOverColor;
        [SerializeField] private Color _mouseDownColor;
        [SerializeField] private Color _SelectedColor;
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _selectedSprite;
        [SerializeField] private Sprite _unSelectedSprite;
        
        private Toggle _toggle;
        private bool _isPressed;
        private bool _isOver;
        private bool _isSelected;

        void UpdateToggleTab()
        {
            Color color = _defaultColor;
            Sprite sprite = _unSelectedSprite;
            
            if (_isSelected)
            {
                color = _SelectedColor;
                sprite = _selectedSprite;
            }
            else if (_isPressed)
                color = _mouseDownColor;            
            else if (_isOver)
                color = _mouseOverColor;
            
            _image.color = color;
            _image.sprite = sprite;
        }
        
        void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _isSelected = _toggle.isOn;
            UpdateToggleTab();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isOver = true;
            UpdateToggleTab();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isOver = false;
            UpdateToggleTab();
        }

        public void OnSelectedChanged(bool isSelected)
        {
            _isSelected = isSelected;
            UpdateToggleTab();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isPressed = false;
            UpdateToggleTab();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPressed = true;
            UpdateToggleTab();
        }
    }
}
