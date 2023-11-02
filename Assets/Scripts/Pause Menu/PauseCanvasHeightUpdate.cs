using UnityEngine;

namespace com.NW84P
{
    public class PauseCanvasHeightUpdate : MonoBehaviour
    {
        [SerializeField]
        private Camera _mainCamera;

        [SerializeField]
        private float _heightOffset;

        private Vector3 _canvasInitalPosition;
        private Transform _thisTransform;
        private Transform _mainCameraTransform;

        public void Awake()
        {
            _canvasInitalPosition = transform.position;
            _mainCameraTransform = _mainCamera.transform;
            _thisTransform = transform;
        }

        public void LateUpdate() => UpdateCanvasHeight();

        private void UpdateCanvasHeight()
            => _thisTransform.position = _canvasInitalPosition + new Vector3(0, _mainCameraTransform.position.y + _heightOffset, 0);

#if UNITY_EDITOR

        public void OnValidate()
        {
            if (_mainCamera == null)
            {
                Debug.LogError("PauseCanvasHeightUpdate: Main Camera Transform is null");
            }
        }

#endif
    }
}
