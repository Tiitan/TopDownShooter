using System.Linq;
using Framework;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Managers
{
    [RequireComponent(typeof(TargetManager))]
    public class LevelManager : SingletonMonoBehavior<LevelManager>
    {
        private GameObject _player;
        public TargetManager TargetManager { get; private set; }
        public OverlayGuiManager OverlayGuiManager { get; private set; }

        public GameObject Player
        {
            get
            {
                // TODO replace lookup when player is instantiated
                if (_player == null)
                    _player = GameObject.FindGameObjectsWithTag("Player").FirstOrDefault();
                return _player;
            }
        }
        
        protected override void Awake()
        {
            base.Awake();

            TargetManager = GetComponent<TargetManager>();
            OverlayGuiManager = GetComponent<OverlayGuiManager>();
        }
        
        [UsedImplicitly]
        public void ExitTriggered(GameObject player)
        {
            if (player.CompareTag("Player"))
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }
}
