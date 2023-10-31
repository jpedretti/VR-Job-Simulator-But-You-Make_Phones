using System;
using UnityEngine;

namespace com.NW84P
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Button _resumeButton;

        [SerializeField]
        private UnityEngine.UI.Button _resetButton;

        [SerializeField]
        private GameObject _gameObjectsParent;

        [SerializeField]
        private GameObject _pauseObjectsParent;

        [SerializeField]
        private ActionBasedControllerManager _rightActionBasedControllerManager;

        [SerializeField]
        private ActionBasedControllerManager _leftActionBasedControllerManager;

        [SerializeField]
        private Transform _myXRTransform;

        private Vector3 _previousPosition;
        private Quaternion _previousRotation;

        public bool ResumePressed { get; set; }

        public bool IsPauseConfigured => _pauseObjectsParent.activeSelf && !_gameObjectsParent.activeSelf;

        public void OnEnable()
        {
            _resumeButton.onClick.AddListener(OnResumePressed);
            _resetButton.onClick.AddListener(OnResetPressed);
        }


        public void OnDisable()
        {
            _resumeButton.onClick.RemoveListener(OnResumePressed);
            _resetButton.onClick.RemoveListener(OnResetPressed);
        }

        private void OnResetPressed()
        {
            var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentScene.name);
        }

        private void OnResumePressed() => ResumePressed = true;

        public void ConfigurePausedState()
        {
            _gameObjectsParent.SetActive(false);
            _pauseObjectsParent.SetActive(true);
            EnableLocomotionActions(false);
            _previousPosition = _myXRTransform.position;
            _previousRotation = _myXRTransform.rotation;
            _myXRTransform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        public void ConfigureUnpausedState()
        {
            ResumePressed = false;
            _gameObjectsParent.SetActive(true);
            _pauseObjectsParent.SetActive(false);
            EnableLocomotionActions(true);
            _myXRTransform.SetPositionAndRotation(_previousPosition, _previousRotation);
        }

        private void EnableLocomotionActions(bool enable)
        {
            _rightActionBasedControllerManager.EnableLocomotionActions(enabled: enable);
            _leftActionBasedControllerManager.EnableLocomotionActions(enabled: enable);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void OnValidate()
        {
            if (_resumeButton == null)
            {
                Debug.LogError("PauseMenu: Resume Button is not set");
            }

            if (_resetButton == null)
            {
                Debug.LogError("PauseMenu: Reset Button is not set");
            }

            if (_gameObjectsParent == null)
            {
                Debug.LogError("PauseMenu: Game Objects Parent is not set");
            }

            if (_pauseObjectsParent == null)
            {
                Debug.LogError("PauseMenu: Pause Objects Parent is not set");
            }

            if (_rightActionBasedControllerManager == null)
            {
                Debug.LogError("PauseMenu: Action Based Controller Manager is not set");
            }

            if (_leftActionBasedControllerManager == null)
            {
                Debug.LogError("PauseMenu: Action Based Controller Manager is not set");
            }

            if (_myXRTransform == null)
            {
                Debug.LogError("PauseMenu: XR Transform is not set");
            }
        }
    }
}
