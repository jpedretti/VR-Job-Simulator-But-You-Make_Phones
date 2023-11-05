namespace com.NW84P
{
    public class GamePaused : BaseGameState
    {
        private IGameState _previousGameState;

        public GamePaused(IGameState previousGameState) => _previousGameState = previousGameState;

        public override IGameState Update(GameStateData gameStateData)
        {
            if (gameStateData.PauseMenu.ResumePressed)
            {
                gameStateData.PauseMenu.ConfigureUnpausedState();
                gameStateData.PauseButton.enabled = true;
                return _previousGameState;
            }
            else if (!gameStateData.PauseMenu.IsPauseConfigured)
            {
                gameStateData.PauseMenu.ConfigurePausedState();
                gameStateData.PauseButton.enabled = false;
            }

            return base.Update(gameStateData);
        }
    }
}
