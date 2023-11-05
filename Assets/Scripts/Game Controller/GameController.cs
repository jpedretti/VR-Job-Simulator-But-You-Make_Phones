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

        [SerializeField]
        private TextMeshProUGUI _messageText;

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
                _timerText.text = "Total Time: 00:00:000";
                _messageText.text = "Press the <color=\"red\">red button</color> to get the rest of the parts.\nPress it again when you finish.\n<size=110%><b>You need to use all the parts to finish the phone.</b></size>";
                _gameStateData = new GameStateData(
                    interactableParts: _interactableParts,
                    timerText: _timerText,
                    pauseMenu: GetComponent<PauseMenu>(),
                    pauseButton: _pauseButton,
                    messageText: _messageText
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
            Debug.Assert(_startButton != null, "Start Button is not set.");
            Debug.Assert(_interactableParts != null, "Interactable Parts is not set.");
            Debug.Assert(_timerText != null, "Timer Text is not set.");
            Debug.Assert(_pauseButton != null, "Pause Button is not set.");
            Debug.Assert(_messageText != null, "Message Text is not set.");
        }

#endif
    }
}
