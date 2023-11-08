using System;
using UnityEngine;

namespace com.NW84P
{
    public class StartController : SceneLoader
    {
        [SerializeField]
        private UnityEngine.UI.Button _startGameButton;

        [SerializeField]
        private UnityEngine.UI.Button _quitGameButton;

        public void Start() => StartCoroutine(FadeOut());

        public void OnEnable()
        {
            _startGameButton.onClick.AddListener(OnStartGameButtonClicked);
            _quitGameButton.onClick.AddListener(OnQuitClicked);
        }

        private void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

        }

        public void OnDisable()
        {
            _startGameButton.onClick.RemoveListener(OnStartGameButtonClicked);
            _quitGameButton.onClick.RemoveListener(OnQuitClicked);
        }

        private void OnStartGameButtonClicked() => StartCoroutine(FadeAndLoadScene(1));

#if UNITY_EDITOR

        public override void OnValidate()
        {
            base.OnValidate();
            Debug.Assert(_startGameButton != null, "Start Game Button is null");
        }

#endif
    }
}
