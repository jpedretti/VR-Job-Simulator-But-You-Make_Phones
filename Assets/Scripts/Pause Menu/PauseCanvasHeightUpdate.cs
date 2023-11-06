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
            => _thisTransform.position = new Vector3(_canvasInitalPosition.x, _mainCameraTransform.position.y + _heightOffset, _canvasInitalPosition.z);

#if UNITY_EDITOR

        public void OnValidate()
        {
            Debug.Assert(_mainCamera != null, "PauseCanvasHeightUpdate: Main Camera is null");
        }

#endif
    }
}
