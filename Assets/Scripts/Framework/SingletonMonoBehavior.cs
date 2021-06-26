using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework
{
    public class SingletonMonoBehavior<T> : MonoBehaviour where T : SingletonMonoBehavior<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"2nd {typeof(T)} init");
                Destroy(gameObject);
                return;
            }
        
            Instance = (T)this;
        }

        protected virtual void OnDestroy()
        {
            Instance = null;
        }
    }
}
