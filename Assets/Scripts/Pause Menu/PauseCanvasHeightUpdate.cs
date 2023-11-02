using UnityEngine;

namespace com.NW84P
{
    public class PauseCanvasHeightUpdate : MonoBehaviour
    {
        [SerializeField]
        private Camera _mainCamera;

        private Vector3 _canvasInitalPosition;

        public void Awake() => _canvasInitalPosition = transform.position;

#if UNITY_EDITOR

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
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
