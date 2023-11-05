using TMPro;
using UnityEngine;

namespace com.NW84P
{
    public class GameEnd : BaseGameState
    {
        private static readonly string[] _END_GAME_FRASES = new string[]
        {
            "Seriously? That slow? You need to do much better than that.",
            "I hope you were just warming up. You need to step up your game.",
            "Is that all you got? You can do better than that.",
            "You are messing with me, right? You want me to congratulate you? You need to get real.",
            "Are you kidding me? That slow? You can do way better than that.",
            "I hope you were just having fun. You need to get serious.",
            "Lame! That slow? You are a waste of time.",
            "I hope you were just teasing me. You need to get real.",
            "I hope you were just playing dumb. You need to amaze me.",
            "Ridiculous! That slow? You are an embarrassment to this game.",
            "Pathetic! That slow? You are a disgrace to this game.",
        };

        public GameEnd(TextMeshProUGUI timerText, TextMeshProUGUI messageText)
        {
            SetColor(timerText, "#008C00");
            SetColor(messageText, "#9A7600");
            messageText.text = _END_GAME_FRASES[Random.Range(0, _END_GAME_FRASES.Length)];
            messageText.fontSize = 10.23f;
        }

        public override IGameState Update(GameStateData gameStateData) => base.Update(gameStateData);

        private static void SetColor(TextMeshProUGUI textMeshPro, string hexColor)
        {
            if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
            {
                textMeshPro.color = color;
            }
        }
    }
}
