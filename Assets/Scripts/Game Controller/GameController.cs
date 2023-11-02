using TMPro;
using UnityEngine;

namespace com.NW84P
{
    [RequireComponent(typeof(PauseMenu))]
    public partial class GameController : MonoBehaviour
    {
        [SerializeField]
        private Button _startButton;

        [SerializeField]
        private GameObject _interactableParts;

        [SerializeField]
        private TextMeshProUGUI _timerText;

        [SerializeField]
        private UnityEngine.UI.Button _pauseButton;

        private bool _pausePressed;
        private IGameState _gameState;
        private GameStateData _gameStateData;

        public bool InsertedSinCard { get; set; } = false;

        public static GameController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                _gameStateData = new GameStateData(
                    interactableParts: _interactableParts,
                    timerText: _timerText,
                    pauseMenu: GetComponent<PauseMenu>(),
                    pauseButton: _pauseButton
                );
                _gameState = new GameStart();
            }
            else
            {
                Destroy(gameObject);
                Debug.LogError("GameController: There can only be one GameController");
            }
        }

        public void OnEnable() => _pauseButton.onClick.AddListener(OnPausePressed);

        public void OnDisable() => _pauseButton.onClick.RemoveListener(OnPausePressed);

        private void OnPausePressed() => _pausePressed = true;

        private void Update()
        {
            _gameStateData.Update(
                buttonPressed: _startButton.IsPressed,
                pauseButtonPressed: _pausePressed,
                insertedSinCard: InsertedSinCard
            );
            _gameState = _gameState.Update(_gameStateData);
            _pausePressed = false;
        }

#if UNITY_EDITOR

        public void OnValidate()
        {
            if (_startButton == null)
            {
                Debug.LogError("Start Button is not set.");
            }

            if (_interactableParts == null)
            {
                Debug.LogError("Interactable Parts is not set.");
            }

            if (_timerText == null)
            {
                Debug.LogError("Timer Text is not set.");
            }

            if (_pauseButton == null)
            {
                Debug.LogError("Pause Button is not set.");
            }
        }

#endif
    }
}
