using System;
using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using static Unity.XR.CoreUtils.XROrigin;

namespace com.NW84P
{
    public class SettingsMenu : MonoBehaviour
    {
        private const float _MIN_SEATED_HEIGHT = 1f;
        private const float _MAX_SEATED_HEIGHT = 2.5f;
        private const float _DEFAULT_SEATED_HEIGHT = 1.36144f;
        private const float _FADE_DURATION = 0.1f;

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

        [SerializeField]
        private SpriteRenderer _SeatedModeFadeSprite;

        [SerializeField]
        private XRRayInteractor[] _xRRayInteractors;

        [SerializeField]
        private Toggle _useComfortVignette;

        [SerializeField]
        private TunnelingVignetteController _tunnelingVignetteController;

        private Color _fadeColor = new(0, 0, 0, 0);
        private bool _isFading;

        public void OnEnable()
        {
            ConfigureSliderSeated();
            _backButton.onClick.AddListener(OnBackPressed);
            _rayToggle.onValueChanged.AddListener(OnRayToggleChanged);
            _seatdModeToggle.onValueChanged.AddListener(OnSeatedModeToggleChanged);
            _seatedModeHeightSlider.onValueChanged.AddListener(OnSeatedModeHeightChanged);
            _useComfortVignette.onValueChanged.AddListener(OnUseVignetteChanged);
        }

        public void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnBackPressed);
            _rayToggle.onValueChanged.RemoveListener(OnRayToggleChanged);
            _seatdModeToggle.onValueChanged.RemoveListener(OnSeatedModeToggleChanged);
            _seatedModeHeightSlider.onValueChanged.RemoveListener(OnSeatedModeHeightChanged);
            _useComfortVignette.onValueChanged.RemoveListener(OnUseVignetteChanged);
        }

        private void OnUseVignetteChanged(bool enable)
        {
            foreach(var provider in _tunnelingVignetteController.locomotionVignetteProviders)
            {
                if (!provider.locomotionProvider.gameObject.CompareTag(Tags.Teleporter))
                {
                    provider.enabled = enable;
                }
            }
        }

        private void OnBackPressed()
        {
            _settingsMenu.SetActive(false);
            _pauseMenu.SetActive(true);
        }

        private void OnRayToggleChanged(bool enable)
        {
            var layerName = enable ? LayerMasks.DEFAULT : LayerMasks.NOTHING;
            var layer = LayerMask.GetMask(layerName);
            for (var i = 0; i < _xRRayInteractors.Length; i++)
            {
                _xRRayInteractors[i].interactionLayers = layer;
            }
        }

        private void OnSeatedModeToggleChanged(bool _) => StartCoroutine(SeatedModeFade());

        private IEnumerator SeatedModeFade()
        {
            // fade in
            StartCoroutine(Fade(duration: _FADE_DURATION / 2, condition: alpha => alpha < 1, alphaFunction: alpha => alpha));
            yield return new WaitUntil(() => !_isFading);

            // change mode
            _xrOrigin.CameraYOffset = _seatedModeHeightSlider.value;
            _xrOrigin.RequestedTrackingOriginMode = _seatdModeToggle.isOn ? TrackingOriginMode.Device : TrackingOriginMode.Floor;
            yield return WaitForSecondsCache.Get(_FADE_DURATION);

            // fade out
            StartCoroutine(Fade(duration: _FADE_DURATION, condition: alpha => alpha > 0, alphaFunction: alpha => 1 - alpha));
            yield return new WaitUntil(() => !_isFading);
        }

        private IEnumerator Fade(float duration, Func<float, bool> condition, Func<float, float> alphaFunction)
        {
            _isFading = true;
            var fadeTimer = 0f;
            while (condition(_fadeColor.a))
            {
                fadeTimer += Time.deltaTime;
                _fadeColor.a = alphaFunction(fadeTimer / duration);
                _SeatedModeFadeSprite.color = _fadeColor;
                yield return null;
            }
            _isFading = false;
        }

        private void OnSeatedModeHeightChanged(float _) => _xrOrigin.CameraYOffset = _seatedModeHeightSlider.value;

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

            if (_SeatedModeFadeSprite == null)
            {
                Debug.LogError("Seated Mode Fade Sprite is null");
            }

            if (_xRRayInteractors == null || _xRRayInteractors.Length != 2)
            {
                Debug.LogError("XR Ray Interactors is null");
            }

            if (_useComfortVignette == null)
            {
                Debug.LogError("Use Comfort Vignette is null");
            }

            if (_tunnelingVignetteController == null)
            {
                Debug.LogError("Tunneling Vignette Controller is null");
            }
        }

#endif
    }
}
