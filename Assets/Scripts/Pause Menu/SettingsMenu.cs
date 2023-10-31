using UnityEngine;
using UnityEngine.UI;

namespace com.NW84P
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject _pauseMenu;

        [SerializeField]
        private GameObject _settingsMenu;

        [SerializeField]
        private UnityEngine.UI.Button _backButton;

        [SerializeField]
        private Toggle _rayToggle;

        [SerializeField]
        private Toggle _seatdModeToggle;

        [SerializeField]
        private Slider _seatedModeHeight;

        public void OnEnable()
        {
            _backButton.onClick.AddListener(OnBackPressed);
            _rayToggle.onValueChanged.AddListener(OnRayToggleChanged);
            _seatdModeToggle.onValueChanged.AddListener(OnSeatedModeToggleChanged);
            _seatedModeHeight.onValueChanged.AddListener(OnSeatedModeHeightChanged);
        }

        public void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnBackPressed);
            _rayToggle.onValueChanged.RemoveListener(OnRayToggleChanged);
            _seatdModeToggle.onValueChanged.RemoveListener(OnSeatedModeToggleChanged);
            _seatedModeHeight.onValueChanged.RemoveListener(OnSeatedModeHeightChanged);
        }

        private void OnBackPressed()
        {
            _settingsMenu.SetActive(false);
            _pauseMenu.SetActive(true);
        }

        private void OnRayToggleChanged(bool enable)
        {
            Debug.Log($"Ray toggle changed to => {enable}");
        }

        private void OnSeatedModeToggleChanged(bool enable)
        {
            Debug.Log($"Seated mode toggle changed to => {enable}");
        }

        private void OnSeatedModeHeightChanged(float height)
        {
            Debug.Log($"Seated mode height changed to => {height}");
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void OnValidate()
        {
            if (_pauseMenu == null)
            {
                Debug.LogError("Pause Menu is null");
            }

            if (_settingsMenu == null)
            {
                Debug.LogError("Settings Menu is null");
            }

            if (_backButton == null)
            {
                Debug.LogError("Back Button is null");
            }

            if (_rayToggle == null)
            {
                Debug.LogError("Ray Toggle is null");
            }

            if (_seatdModeToggle == null)
            {
                Debug.LogError("Seated Mode Toggle is null");
            }

            if (_seatedModeHeight == null)
            {
                Debug.LogError("Seated Mode Height is null");
            }
        }
    }
}
