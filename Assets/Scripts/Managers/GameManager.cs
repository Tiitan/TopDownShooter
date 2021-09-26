using Framework;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : SingletonMonoBehavior<GameManager>
    {
        public Inventory Inventory { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;
            
            DontDestroyOnLoad(gameObject);
            SceneManager.UnloadSceneAsync("Init");
            
            Inventory = GetComponent<Inventory>();
        }
    }
}
