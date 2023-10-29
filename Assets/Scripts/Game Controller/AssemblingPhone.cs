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
                return new GameEnd();
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
            var timer = string.Format("{0:D2}:{1:D2}:{2:D3}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
            if (timeSpan.Hours > 0)
            {
                timer = $"\n{timeSpan.Hours:D2}:{timer}";
            }

            timerText.text = $"Total Time: {timer}";
        }
    }
}
