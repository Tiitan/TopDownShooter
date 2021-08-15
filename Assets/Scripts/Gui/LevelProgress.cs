using System.ComponentModel;
using Enums;
using Managers;
using TMPro;
using UnityEngine;

namespace Gui
{
    public class LevelProgress : MonoBehaviour
    {
        [SerializeField] private ProgressBar _progressBar;
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private Color _finishColor;
        private LevelManager _levelManager;

        void Start()
        {
            _levelManager = LevelManager.Instance;
            _levelManager.PropertyChanged += InstanceOnPropertyChanged;

            UpdateText();
            _progressBar.UpdateProgress(0);
        }

        /// <summary>Called when changing wave.</summary>
        private void UpdateText()
        {
            _progressText.text = $"Wave: {_levelManager.CurrentWave} / {_levelManager.WaveCount}";
        }

        /// <summary>Called after each kill</summary>
        private void UpdateProgress()
        {
            float waveProgress = (float)_levelManager.WaveMobKill / _levelManager.WaveMobsCount;
            _progressBar.UpdateProgress((_levelManager.CurrentWave + waveProgress - 1) / _levelManager.WaveCount);
        }
        
        private void InstanceOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_levelManager.CurrentWave) ||
                e.PropertyName == nameof(_levelManager.WaveCount))
            {
                UpdateText();
            }
            
            if (e.PropertyName == nameof(_levelManager.WaveMobKill))
            {
                UpdateProgress();
            }
            
            if (e.PropertyName == nameof(_levelManager.LevelState) && _levelManager.LevelState == LevelState.Finish)
            {
                _progressBar.Image.color = _finishColor;
            }
        }
        
        private void OnDestroy()
        {
            if (_levelManager != null)
                _levelManager.PropertyChanged -= InstanceOnPropertyChanged;
        }
    }
}
