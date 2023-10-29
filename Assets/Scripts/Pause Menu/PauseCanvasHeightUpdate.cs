using UnityEngine;

namespace com.NW84P
{
    public class PauseCanvasHeightUpdate : MonoBehaviour
    {
        [SerializeField]
        private Camera _mainCamera;

        private Vector3 _canvasInitalPosition;

        public void Awake() => _canvasInitalPosition = transform.position;

        public void OnEnable()
            => transform.position = _canvasInitalPosition + new Vector3(0, _mainCamera.transform.position.y, 0);

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void OnValidate()
        {
            if (_mainCamera == null)
            {
                Debug.LogError("PauseCanvasHeightUpdate: Main Camera Transform is null");
            }
        }
    }
}
