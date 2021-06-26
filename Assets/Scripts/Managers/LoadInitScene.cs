using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    /// <summary>
    /// Allow running game from any scene.
    /// </summary>
    public class LoadInitScene : MonoBehaviour
    {
        void Awake()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogWarning($"Missing static GameManager Instance, reload 'Init' scene");
                SceneManager.LoadScene("Init", LoadSceneMode.Additive);
            }
        }
    }
}
