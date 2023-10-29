namespace com.NW84P
{
    public abstract class BaseGameState : IGameState
    {
        public virtual IGameState Update(GameStateData gameStateData)
        {
            if (gameStateData.PauseButtonPressed)
            {
                return new GamePaused(this);
            }

            return this;
        }
    }
}
