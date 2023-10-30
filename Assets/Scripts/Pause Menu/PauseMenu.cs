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

        public bool ResumePressed { get; set; }

        public GameObject GameObjectsParent => _gameObjectsParent;

        public GameObject PauseObjectsParent => _pauseObjectsParent;

        public void OnEnable() => _resumeButton.onClick.AddListener(OnResumePressed);

        public void OnDisable() => _resumeButton.onClick.RemoveListener(OnResumePressed);

        // Disable teleportation when the pause menu is open
        public void EnableLocomotionActions(bool enable)
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

        private void OnResumePressed() => ResumePressed = true;

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
        }
    }
}
