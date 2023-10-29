using UnityEngine;

namespace com.NW84P
{
    public class GamePaused : BaseGameState
    {
        private IGameState _previousGameState;
        private Vector3 _previousPosition;
        private Quaternion _previousRotation;

        public GamePaused(IGameState previousGameState) => _previousGameState = previousGameState;

        public override IGameState Update(GameStateData gameStateData)
        {
            if (gameStateData.PauseMenu.ResumePressed)
            {
                gameStateData.PauseMenu.ResumePressed = false;
                gameStateData.PauseMenu.PauseObjectsParent.SetActive(false);
                gameStateData.PauseMenu.GameObjectsParent.SetActive(true);
                gameStateData.LocomotionSystem.SetActive(true);
                gameStateData.MyXRTransform.SetPositionAndRotation(_previousPosition, _previousRotation);
                gameStateData.PauseButton.enabled = true;
                return _previousGameState;
            }
            else if (!gameStateData.PauseMenu.PauseObjectsParent.activeSelf)
            {
                gameStateData.PauseMenu.GameObjectsParent.SetActive(false);
                gameStateData.PauseMenu.PauseObjectsParent.SetActive(true);
                gameStateData.LocomotionSystem.SetActive(false);
                _previousPosition = gameStateData.MyXRTransform.position;
                _previousRotation = gameStateData.MyXRTransform.rotation;
                gameStateData.MyXRTransform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                gameStateData.PauseButton.enabled = false;
            }

            return base.Update(gameStateData);
        }
    }
}
