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
        public TargetManager TargetManager { get; private set; }
        public OverlayGuiManager OverlayGuiManager { get; private set; }

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
