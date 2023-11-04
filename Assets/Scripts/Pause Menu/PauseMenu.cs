using UnityEngine;

namespace com.NW84P
{
    public class PauseMenu : MonoBehaviour
    {
        private const float _CAMERA_MENU_DISTANCE = -1.6f;

        [SerializeField]
        private UnityEngine.UI.Button _resumeButton;

        [SerializeField]
        private UnityEngine.UI.Button _resetButton;

        [SerializeField]
        private UnityEngine.UI.Button _settingsButton;

        [SerializeField]
        private UnityEngine.UI.Button _mainMenuButton;

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
        private GameObject _settingsMenuUI;

        [SerializeField]
        private SettingsMenu _settingsMenu;

        private Vector3 _previousPosition;
        private Quaternion _previousRotation;

        public bool ResumePressed { get; set; }

        public bool IsPauseConfigured => _pauseObjectsParent.activeSelf;

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
            _settingsMenuUI.SetActive(true);
            _pauseMenu.SetActive(false);
        }

        private void OnMainMenuPressed()
        {
            Debug.Log("Main Menu Pressed");
        }

        public void ConfigurePausedState()
        {
            _pauseObjectsParent.SetActive(true);
            EnableLocomotionActions(false);
            _previousPosition = _myXRTransform.position;
            _previousRotation = _myXRTransform.rotation;
            _settingsMenu.DisableRays();
            _myXRTransform.SetPositionAndRotation(new(0, 0, _CAMERA_MENU_DISTANCE), Quaternion.identity);
        }

        public void ConfigureUnpausedState()
        {
            ResumePressed = false;
            _pauseObjectsParent.SetActive(false);
            EnableLocomotionActions(true);
            _myXRTransform.SetPositionAndRotation(_previousPosition, _previousRotation);
            _settingsMenu.TogglesRays();
        }

        private void EnableLocomotionActions(bool enable)
        {
            _rightActionBasedControllerManager.EnableLocomotionActions(enabled: enable);
            _leftActionBasedControllerManager.EnableLocomotionActions(enabled: enable);
        }

#if UNITY_EDITOR

        public void OnValidate()
        {
            Debug.Assert(_resumeButton != null, "PauseMenu: Resume Button is not set");
            Debug.Assert(_resetButton != null, "PauseMenu: Reset Button is not set");
            Debug.Assert(_settingsButton != null, "PauseMenu: Settings Button is not set");
            Debug.Assert(_mainMenuButton != null, "PauseMenu: Main Menu Button is not set");
            Debug.Assert(_pauseObjectsParent != null, "PauseMenu: Pause Objects Parent is not set");
            Debug.Assert(_rightActionBasedControllerManager != null, "PauseMenu: Action Based Controller Manager is not set");
            Debug.Assert(_leftActionBasedControllerManager != null, "PauseMenu: Action Based Controller Manager is not set");
            Debug.Assert(_myXRTransform != null, "PauseMenu: XR Transform is not set");
            Debug.Assert(_pauseMenu != null, "PauseMenu: Pause Menu is not set");
            Debug.Assert(_settingsMenuUI != null, "PauseMenu: Settings Menu UI is not set");
            Debug.Assert(_settingsMenu != null, "PauseMenu: Settings Menu is not set");
        }

#endif
    }
}
