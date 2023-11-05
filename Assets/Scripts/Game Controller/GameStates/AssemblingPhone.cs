using System;
using TMPro;
using UnityEngine;

namespace com.NW84P
{
    public class AssemblingPhone : BaseGameState
    {
        private float _timer;

        public override IGameState Update(GameStateData gameStateData)
        {
            if (gameStateData.ButtonPressed && gameStateData.InsertedSinCard)
            {
                return new GameEnd(gameStateData.TimerText, gameStateData.MessageText);
            }
            else
            {
                _timer += Time.deltaTime;
                UpdateTimeText(gameStateData.TimerText);
            }

            return base.Update(gameStateData);
        }

        private void UpdateTimeText(TextMeshProUGUI timerText)
        {
            var timeSpan = TimeSpan.FromSeconds(_timer);
            string timer;
            if (timeSpan.Hours > 0)
            {
                timer = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            }
            else
            {
                timer = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}:{timeSpan.Milliseconds:D3}";
            }

            timerText.text = $"Total Time: {timer}";
        }
    }
}
