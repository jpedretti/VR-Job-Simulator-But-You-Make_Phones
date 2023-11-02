using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using static Unity.XR.CoreUtils.XROrigin;

namespace com.NW84P
{
    public class SettingsMenu : MonoBehaviour
    {
        private const float _MIN_SEATED_HEIGHT = 1f;
        private const float _MAX_SEATED_HEIGHT = 2.5f;
        private const float _DEFAULT_SEATED_HEIGHT = 1.36144f;

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
        private Slider _seatedModeHeightSlider;

        [SerializeField]
        private XROrigin _xrOrigin;

        [SerializeField]
        private PauseCanvasHeightUpdate _canvasHeightUpdate;

        public void OnEnable()
        {
            ConfigureSliderSeated();
            _backButton.onClick.AddListener(OnBackPressed);
            _rayToggle.onValueChanged.AddListener(OnRayToggleChanged);
            _seatdModeToggle.onValueChanged.AddListener(OnSeatedModeToggleChanged);
            _seatedModeHeightSlider.onValueChanged.AddListener(OnSeatedModeHeightChanged);
        }

        public void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnBackPressed);
            _rayToggle.onValueChanged.RemoveListener(OnRayToggleChanged);
            _seatdModeToggle.onValueChanged.RemoveListener(OnSeatedModeToggleChanged);
            _seatedModeHeightSlider.onValueChanged.RemoveListener(OnSeatedModeHeightChanged);
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

        private void OnSeatedModeToggleChanged(bool _)
        {
            _xrOrigin.CameraYOffset = _seatedModeHeightSlider.value;
            _xrOrigin.RequestedTrackingOriginMode = _seatdModeToggle.isOn ? TrackingOriginMode.Device : TrackingOriginMode.Floor;
        }

        private void OnSeatedModeHeightChanged(float _)
        {
            _xrOrigin.CameraYOffset = _seatedModeHeightSlider.value;
        }

        private void ConfigureSliderSeated()
        {
            _seatedModeHeightSlider.minValue = _MIN_SEATED_HEIGHT;
            _seatedModeHeightSlider.maxValue = _MAX_SEATED_HEIGHT;
            if (!_seatdModeToggle.isOn) _seatedModeHeightSlider.value = _DEFAULT_SEATED_HEIGHT;
        }

#if UNITY_EDITOR

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

            if (_seatedModeHeightSlider == null)
            {
                Debug.LogError("Seated Mode Height is null");
            }

            if (_xrOrigin == null)
            {
                Debug.LogError("XR Origin is null");
            }

            if (_canvasHeightUpdate == null)
            {
                Debug.LogError("Canvas Height Update is null");
            }
        }

#endif
    }
}
