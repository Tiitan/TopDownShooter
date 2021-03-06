using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Enums;
using Framework;
using Interface;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    [RequireComponent(typeof(TargetManager))]
    public class LevelManager : SingletonMonoBehavior<LevelManager>, INotifyPropertyChanged
    {
        [SerializeField] private float _timeBeforeFirstWave = 1; 
        [SerializeField] private float _timeBetweenWaves = 3; 
        [SerializeField] private float _gameOverDuration = 3; 
        [SerializeField] private GameObject _pausePanel; 
        
        private GameObject _player;
        public TargetManager TargetManager { get; private set; }
        public OverlayGuiManager OverlayGuiManager { get; private set; }

        private int _currentWave;
        private int _waveCount;
        private int _mobCount;
        private int _waveMobsCount;
        private int _waveMobKill;
        private int _levelMobKill;
        private LevelState _levelState;

        private List<ISpawner> _spawners;
        private readonly List<GameObject> _mobs = new List<GameObject>();

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>level state: Start, Wave, InterWave, Finish, PlayerDead</summary>
        public LevelState LevelState
        {
            get => _levelState;
            private set
            {
                _levelState = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Current wave in progress</summary>
        public int CurrentWave
        {
            get => _currentWave;
            private set
            {
                _currentWave = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>Total number of waves.</summary>
        public int WaveCount
        {
            get => _waveCount;
            private set
            {
                _waveCount = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>Current number of mobs alive. wave finished when reach 0.</summary>
        public int MobCount
        {
            get => _mobCount;
            private set
            {
                _mobCount = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>Total number of mobs spawned at the current wave.</summary>
        public int WaveMobsCount
        {
            get => _waveMobsCount;
            private set
            {
                _waveMobsCount = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>Number of killed mobs at this wave.</summary>
        public int WaveMobKill
        {
            get => _waveMobKill;
            private set
            {
                _waveMobKill = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>Total count of mob killed on this level.</summary>
        public int LevelMobKill
        {
            get => _levelMobKill;
            private set
            {
                _levelMobKill = value;
                OnPropertyChanged();
            }
        }
        

        /// <summary>Spawners cache</summary>
        private List<ISpawner> Spawners
        {
            get
            {
                _spawners ??= GameObject.FindGameObjectsWithTag("Spawner")
                    .Select(x => x.GetComponent<ISpawner>()).ToList();
                return _spawners;
            }
        }
        
        /// <summary>Player cache</summary>
        public GameObject Player
        {
            get
            {
                // TODO replace lookup when player is instantiated
                _player ??= GameObject.FindGameObjectsWithTag("Player").FirstOrDefault();
                return _player;
            }
        }
        
        protected override void Awake()
        {
            base.Awake();

            TargetManager = GetComponent<TargetManager>();
            OverlayGuiManager = GetComponent<OverlayGuiManager>();
        }

        private IEnumerator Start()
        {
            LevelState = LevelState.Start;
            WaveCount = Spawners.Select(x => x.Waves.Max()).Max();
            Debug.Log($"Level started, waves: {_waveCount}.");
            yield return new WaitForSeconds(_timeBeforeFirstWave);
            SpawnNextWave();
        }

        public void RegisterMob(GameObject mob)
        {
            _mobs.Add(mob);
        }

        public void UnRegisterMob(GameObject mob)
        {
            WaveMobKill += 1;
            LevelMobKill += 1;
            
            _mobs.Remove(mob);
            if (_mobs.Count == 0)
                StartCoroutine(WaveOver());
        }

        public void OnPlayerDie()
        {
            StartCoroutine(GameOver());
        }
        
        public void ExitTriggered(GameObject player)
        {
            if (player.CompareTag("Player") && _levelState == LevelState.Finish)
            {
                // TODO win logic
                SceneManager.LoadScene("Menu");
            }
        }

        private void SpawnNextWave()
        {
            if (LevelState != LevelState.InterWave && LevelState != LevelState.Start)
            {
                Debug.LogWarning($"LevelManager's SpawnNextWave in wrong state ({LevelState}).");
                return; // Player died just before killing the last mob ?
            }

            LevelState = LevelState.Wave;
            CurrentWave += 1;

            Debug.Log($"Starting wave {_currentWave}.");
            var currentWaveSpawners = Spawners.Where(x => x.Waves.Contains(_currentWave)).ToList();

            int mobs = 0;
            foreach (ISpawner spawner in currentWaveSpawners)
                mobs += spawner.Spawn(_currentWave);
            WaveMobsCount = mobs;
            WaveMobKill = 0; // after WaveMobsCount, trigger GUI.LevelProgress update
            
            if (!currentWaveSpawners.Any())
            {
                Debug.LogWarning($"Wave {_currentWave} missing spawner. skipped.");
                StartCoroutine(WaveOver());
            }
        }

        // Resharper disable Unity.PerformanceAnalysis
        private IEnumerator WaveOver()
        {
            if (LevelState != LevelState.Wave)
            {
                Debug.LogWarning($"LevelManager's WaveOver. in wrong state ({LevelState}).");
                yield break; // Player died just before killing the last mob ?
            }
            
            if (_currentWave >= _waveCount)
            {
                Debug.Log("Last wave finished.");
                LevelState = LevelState.Finish;
                yield break;
            }
            
            LevelState = LevelState.InterWave;
            Debug.Log($"Wave {_currentWave} finished.");
            yield return new WaitForSeconds(_timeBetweenWaves);
            SpawnNextWave();
        }

        private IEnumerator GameOver(bool skipDelay = false)
        {
            LevelState = LevelState.PlayerDead;
            Debug.Log("Game over.");
            if (!skipDelay)
                yield return new WaitForSeconds(_gameOverDuration);
            // TODO loose logic
            SceneManager.LoadScene("Menu");
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Pause(bool pause)
        {
            Time.timeScale = pause ? 0 : 1;
            _pausePanel.SetActive(pause);
        }

        // Called by "Leave" button in pause panel. Trigger an immediate game over
        public void LeaveLevelButton()
        {
            Pause(false);
            StartCoroutine(GameOver(skipDelay: true));
        }
        
        // Called when The game is minimized and restored. open the pause menu but don't auto restart the game.
        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                Pause(true);
        }
    }
}
