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
        private UnityEngine.UI.Button _settingsButton;

        [SerializeField]
        private UnityEngine.UI.Button _mainMenuButton;

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

        [SerializeField]
        private GameObject _pauseMenu;

        [SerializeField]
        private GameObject _settingsMenu;

        private Vector3 _previousPosition;
        private Quaternion _previousRotation;

        public bool ResumePressed { get; set; }

        public bool IsPauseConfigured => _pauseObjectsParent.activeSelf && !_gameObjectsParent.activeSelf;

        public void OnEnable()
        {
            _resumeButton.onClick.AddListener(OnResumePressed);
            _resetButton.onClick.AddListener(OnResetPressed);
            _settingsButton.onClick.AddListener(OnSettingsPressed);
            _mainMenuButton.onClick.AddListener(OnMainMenuPressed);
        }

        public void OnDisable()
        {
            _resumeButton.onClick.RemoveListener(OnResumePressed);
            _resetButton.onClick.RemoveListener(OnResetPressed);
            _settingsButton.onClick.RemoveListener(OnSettingsPressed);
            _mainMenuButton.onClick.RemoveListener(OnMainMenuPressed);
        }

        private void OnResetPressed()
        {
            var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentScene.name);
        }

        private void OnResumePressed() => ResumePressed = true;

        private void OnSettingsPressed()
        {
            _settingsMenu.SetActive(true);
            _pauseMenu.SetActive(false);
        }

        private void OnMainMenuPressed()
        {
            Debug.Log("Main Menu Pressed");
        }

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

#if UNITY_EDITOR

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

            if (_settingsButton == null)
            {
                Debug.LogError("PauseMenu: Settings Button is not set");
            }

            if (_mainMenuButton == null)
            {
                Debug.LogError("PauseMenu: Main Menu Button is not set");
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

#endif
    }
}
