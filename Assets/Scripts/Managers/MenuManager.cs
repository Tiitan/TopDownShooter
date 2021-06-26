using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class MenuManager : MonoBehaviour
    {
        [UsedImplicitly]
        public void LoadLevel(string sceneId)
        {
            SceneManager.LoadScene(sceneId);
        }
    }
}
