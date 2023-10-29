using UnityEngine;

namespace com.NW84P
{
    public abstract class BaseGameState : IGameState
    {
        public virtual IGameState Update(GameStateData gameStateData)
        {
            if (gameStateData.PauseButtonPressed)
            {
                // pause game
                Debug.Log("Pausing game on BaseGameState");
                //return new GamePaused(this);
            }
            return this;
        }
    }
}
