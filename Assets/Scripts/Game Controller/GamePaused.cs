using UnityEngine;

namespace com.NW84P
{
    public class GamePaused : BaseGameState
    {
        private IGameState _previousGameState;

        public GamePaused(IGameState previousGameState)
        {
            Debug.Log("Pausing game");
            _previousGameState = previousGameState;
            //Time.timeScale = 0f;
        }

        public override IGameState Update(GameStateData gameStateData)
        {
            // if UI close pause menu button pressed return previous game state
            if (gameStateData.PauseMenuClosed)
            {
                // unpause game
                Debug.Log("Unpausing game");
                Time.timeScale = 1f;
                return _previousGameState;
            }
            return base.Update(gameStateData);
        }
    }
}
