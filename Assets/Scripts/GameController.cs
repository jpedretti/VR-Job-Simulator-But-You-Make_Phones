using UnityEngine;

namespace com.NW84P
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private Button _startButton;

        [SerializeField]
        private GameObject[] _objectsToEnable;

        private float _timer;
        private bool _buttonPressed;
        private IGameState _gameState;

        public bool InsertedSinCard { get; set; } = false;

        public static GameController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogError("GameController: There can only be one GameController");
            }

            _gameState = new GameStart();
        }

        public void OnEnable() => _startButton.OnButtonPressed.AddListener(ButtonPressed);

        public void OnDisable() => _startButton.OnButtonPressed.RemoveListener(ButtonPressed);

        private void ButtonPressed() => _buttonPressed = true;

        private void Update()
        {
            _gameState = _gameState.Update();
            _buttonPressed = false;
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void OnValidate()
        {
            if (_startButton == null)
            {
                Debug.LogError("Start Button is not set.");
            }

            if (_objectsToEnable == null || _objectsToEnable.Length == 0)
            {
                Debug.LogError("Objects to Enable is not set.");
            }
        }

        private class GameStart : IGameState
        {
            public GameStart() => Instance._timer = 0f;

            public IGameState Update()
            {
                if (Instance._buttonPressed)
                {
                    foreach (var obj in Instance._objectsToEnable)
                    {
                        obj.SetActive(true);
                    }

                    return new AssemblingPhone();
                }
                return this;
            }
        }

        private class AssemblingPhone : IGameState
        {
            public IGameState Update()
            {
                if (Instance._buttonPressed && Instance.InsertedSinCard)
                {
                    return new GameEnd();
                }
                else
                {
                    Instance._timer += Time.deltaTime;
                }

                return this;
            }
        }

        private class GameEnd : IGameState
        {
            public GameEnd() => Debug.Log($"Game ended. Time: {Instance._timer}");

            public IGameState Update() => this;
        }

        private class GamePaused : IGameState
        {
            private IGameState _previousGameState;

            public GamePaused(IGameState previousGameState) => _previousGameState = previousGameState;

            public IGameState Update()
            {
                // if UI close pause menu button pressed return previous game state
                return this;
            }
        }
    }

    public interface IGameState
    {
        IGameState Update();
    }
}
