using UnityEngine;

namespace com.NW84P
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private Button _startButton;

        [SerializeField]
        private GameObject[] _objectsToEnable;

        private bool _isGameStarted = false;
        private bool _isGameEnded = false;
        private float _timer = 0f;

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
        }

        private void Start() => _startButton.OnButtonPressed.AddListener(ButtonPressed);

        private void ButtonPressed()
        {
            if (!_isGameStarted)
            {
                _isGameStarted = true;
                foreach (var obj in _objectsToEnable)
                {
                    obj.SetActive(true);
                }
            }
            if (InsertedSinCard)
            {
                _isGameEnded = true;
                Debug.Log($"Game ended. Time: {_timer}");
            }
        }

        private void Update()
        {
            if (_isGameStarted && !_isGameEnded)
            {
                _timer += Time.deltaTime;
            }
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
    }
}
