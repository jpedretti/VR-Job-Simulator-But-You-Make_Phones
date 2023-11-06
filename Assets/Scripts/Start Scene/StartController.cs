using UnityEngine;

namespace com.NW84P
{
    public class StartController : SceneLoader
    {
        [SerializeField]
        private UnityEngine.UI.Button _startGameButton;

        public void Start() => StartCoroutine(FadeOut());

        public void OnEnable() => _startGameButton.onClick.AddListener(OnStartGameButtonClicked);

        public void OnDisable() => _startGameButton.onClick.RemoveListener(OnStartGameButtonClicked);

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
