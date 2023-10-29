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
            GameObject locomotionSystem,
            Transform myXRTransform)
        {
            InteractableParts = interactableParts;
            TimerText = timerText;
            PauseMenu = pauseMenu;
            LocomotionSystem = locomotionSystem;
            MyXRTransform = myXRTransform;
            ButtonPressed = false;
            PauseButtonPressed = false;
            InsertedSinCard = false;
        }

        public bool ButtonPressed { get; private set; }
        public bool PauseButtonPressed { get; private set; }
        public bool InsertedSinCard { get; private set; }
        public GameObject InteractableParts { get; private set; }
        public TextMeshProUGUI TimerText { get; private set; }
        public PauseMenu PauseMenu { get; private set; }
        public GameObject LocomotionSystem { get; private set; }
        public Transform MyXRTransform { get; private set; }

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
