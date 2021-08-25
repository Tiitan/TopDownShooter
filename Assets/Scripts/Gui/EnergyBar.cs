using System.Collections.Generic;
using System.ComponentModel;
using Characters;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
    public class EnergyBar : MonoBehaviour
    {
        [SerializeField] private GameObject _energySegmentPrefab;
        [SerializeField] private Color emptyColor;
        [SerializeField] private Color fullColor;
        
        private Character _character;
        
        private readonly List<Image> _energySegments = new List<Image>();

        public void Start()
        {
            _character = LevelManager.Instance.Player.GetComponent<Character>();
            _character.PropertyChanged += CharacterOnPropertyChanged;
            UpdateMaxEnergy(_character.MaxEnergy);
            UpdateEnergy(_character.Energy);
        }

        private void UpdateEnergy(int value)
        {
            for (int i = 0; i < _energySegments.Count; i++)
                _energySegments[i].color = i < value ? fullColor : emptyColor;
        }
        
        private void UpdateMaxEnergy(int value)
        {
            while (_energySegments.Count > value)
            {
                Destroy(_energySegments[_energySegments.Count - 1].gameObject);
                _energySegments.RemoveAt(_energySegments.Count - 1);
            }
            while (_energySegments.Count < value)
                _energySegments.Add(Instantiate(_energySegmentPrefab, transform).GetComponent<Image>());
        }
        
        private void CharacterOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var character = (Character) sender;
            switch (e.PropertyName)
            {
                case nameof(Character.Energy): UpdateEnergy(character.Energy); break;
                case nameof(Character.MaxEnergy): UpdateMaxEnergy(character.MaxEnergy); break;
            }
        }

        private void OnDestroy()
        {
            if (_character != null)
                _character.PropertyChanged -= CharacterOnPropertyChanged;
        }
    }
}
