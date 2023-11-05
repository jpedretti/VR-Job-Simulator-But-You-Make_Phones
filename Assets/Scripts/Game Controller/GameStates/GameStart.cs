namespace com.NW84P
{
    public class GameStart : BaseGameState
    {
        public override IGameState Update(GameStateData gameStateData)
        {
            if (gameStateData.ButtonPressed)
            {
                gameStateData.InteractableParts.SetActive(true);
                return new AssemblingPhone();
            }

            return base.Update(gameStateData);
        }
    }
}
