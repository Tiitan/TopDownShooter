using Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : SingletonMonoBehavior<GameManager>
    {
        protected override void Awake()
        {
            base.Awake();
            if (Instance == this)
            {
                DontDestroyOnLoad(gameObject);
                SceneManager.UnloadSceneAsync("Init");
            }
        }
    }
}
