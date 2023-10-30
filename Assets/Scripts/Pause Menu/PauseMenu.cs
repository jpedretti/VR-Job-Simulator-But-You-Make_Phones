using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

namespace com.NW84P
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Button _resumeButton;

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

        public void OnEnable() => _resumeButton.onClick.AddListener(OnResumePressed);

        public void OnDisable() => _resumeButton.onClick.RemoveListener(OnResumePressed);

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

        private void OnResumePressed() => ResumePressed = true;

        private void EnableLocomotionActions(bool enable)
        {
            if (enable)
            {
                _rightActionBasedControllerManager.EnableLocomotionActions(teleportActivate: true, teleportCancel: true, turn: true, snapTurn: true);
                _leftActionBasedControllerManager.EnableLocomotionActions(move: true);
            }
            else
            {
                _rightActionBasedControllerManager.DisableAllLocomotionActions();
                _leftActionBasedControllerManager.DisableAllLocomotionActions();
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void OnValidate()
        {
            if (_resumeButton == null)
            {
                Debug.LogError("PauseMenu: Resume Button is not set");
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
