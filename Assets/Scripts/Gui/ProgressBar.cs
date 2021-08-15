using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
    [RequireComponent(typeof(Image), typeof(RectTransform))]
    public class ProgressBar : MonoBehaviour
    {
        private Image _image;
        private RectTransform _rectTransform;

        private float _maxWidth;
        
        public Image Image => _image;
        
        void Awake()
        {
            _image = GetComponent<Image>();
            _rectTransform = (RectTransform) transform;

            _maxWidth = _rectTransform.rect.width;
        }

        public void UpdateProgress(float progress)
        {
            progress = float.IsNaN(progress) ? 0 : Mathf.Clamp(progress, 0, 1);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _maxWidth * progress);
        }
    }
}
