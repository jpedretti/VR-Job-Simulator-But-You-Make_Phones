using TMPro;
using UnityEngine;

namespace com.NW84P
{
    public struct GameStateData
    {
        public GameStateData(
            GameObject interactableParts,
            TextMeshProUGUI timerText,
            PauseMenu pauseMenu,
            Transform myXRTransform,
            UnityEngine.UI.Button pauseButton)
        {
            InteractableParts = interactableParts;
            TimerText = timerText;
            PauseMenu = pauseMenu;
            MyXRTransform = myXRTransform;
            ButtonPressed = false;
            PauseButtonPressed = false;
            InsertedSinCard = false;
            PauseButton = pauseButton;
        }

        public bool ButtonPressed { get; private set; }
        public bool PauseButtonPressed { get; private set; }
        public bool InsertedSinCard { get; private set; }
        public GameObject InteractableParts { get; private set; }
        public TextMeshProUGUI TimerText { get; private set; }
        public PauseMenu PauseMenu { get; private set; }
        public Transform MyXRTransform { get; private set; }
        public UnityEngine.UI.Button PauseButton { get; private set; }

        public void Update(
            bool buttonPressed,
            bool pauseButtonPressed,
            bool insertedSinCard)
        {
            ButtonPressed = buttonPressed;
            PauseButtonPressed = pauseButtonPressed;
            InsertedSinCard = insertedSinCard;
        }
    }
}
