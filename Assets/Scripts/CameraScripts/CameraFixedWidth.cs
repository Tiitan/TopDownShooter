using Cinemachine;
using UnityEngine;

namespace CameraScripts
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CameraFixedWidth : MonoBehaviour
    {
        [SerializeField] float _horizontalFov;

        private CinemachineVirtualCamera _virtualCamera;
    
        private void Awake()
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _virtualCamera.m_Lens.OrthographicSize = _horizontalFov / ((float)Screen.width / Screen.height);
        }

#if UNITY_EDITOR
        void Update ()
        {
            _virtualCamera.m_Lens.OrthographicSize = _horizontalFov / ((float)Screen.width / Screen.height);
        }
#endif //UNITY_EDITOR
    }
}