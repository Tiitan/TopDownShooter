using System.ComponentModel;
using Interface;
using JetBrains.Annotations;
using UnityEngine;

namespace Gui
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private ProgressBar _progress;
        
        private IDamageable _damageable;
        
        [UsedImplicitly]
        public void Init(IDamageable damageable)
        {
            _damageable = damageable;
            damageable.PropertyChanged += DamageableOnPropertyChanged;
            _progress.UpdateProgress(_damageable.HealthPoint / _damageable.MaxHealthPoint);
        }

        private void DamageableOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_damageable.HealthPoint) ||
                e.PropertyName == nameof(_damageable.MaxHealthPoint))
            {
                _progress.UpdateProgress(_damageable.HealthPoint / _damageable.MaxHealthPoint);
            }
        }

        private void OnDestroy()
        {
            if (_damageable != null)
                _damageable.PropertyChanged -= DamageableOnPropertyChanged;
        }
    }
}
