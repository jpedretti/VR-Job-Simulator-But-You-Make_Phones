﻿using TMPro;
using UnityEngine;

namespace com.NW84P
{
    public struct GameStateData
    {
        public GameStateData(
            GameObject interactableParts,
            TextMeshProUGUI timerText,
            PauseMenu pauseMenu,
            UnityEngine.UI.Button pauseButton,
            TextMeshProUGUI messageText)
        {
            InteractableParts = interactableParts;
            TimerText = timerText;
            PauseMenu = pauseMenu;
            ButtonPressed = false;
            PauseButtonPressed = false;
            InsertedSinCard = false;
            PauseButton = pauseButton;
            MessageText = messageText;
        }

        public bool ButtonPressed { get; private set; }
        public bool PauseButtonPressed { get; private set; }
        public bool InsertedSinCard { get; private set; }
        public GameObject InteractableParts { get; private set; }
        public TextMeshProUGUI TimerText { get; private set; }
        public TextMeshProUGUI MessageText { get; private set; }
        public PauseMenu PauseMenu { get; private set; }
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
