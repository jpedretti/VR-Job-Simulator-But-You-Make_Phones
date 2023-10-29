using TMPro;
using UnityEngine;

namespace com.NW84P
{
    public struct GameStateData
    {
        public bool ButtonPressed { get; private set; }
        public bool PauseButtonPressed { get; private set; }
        public bool PauseMenuClosed { get; private set; }
        public bool InsertedSinCard { get; private set; }
        public GameObject InteractableParts { get; private set; }
        public TextMeshProUGUI TimerText { get; private set; }

        public void Update(
            bool buttonPressed,
            bool pauseButtonPressed,
            bool pauseMenuClosed,
            bool insertedSinCard,
            GameObject interactableParts,
            TextMeshProUGUI timerText)
        {
            ButtonPressed = buttonPressed;
            PauseButtonPressed = pauseButtonPressed;
            PauseMenuClosed = pauseMenuClosed;
            InsertedSinCard = insertedSinCard;
            InteractableParts = interactableParts;
            TimerText = timerText;
        }
    }
}
