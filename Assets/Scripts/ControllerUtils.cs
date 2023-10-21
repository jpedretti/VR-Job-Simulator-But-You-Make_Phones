using UnityEngine.XR.Interaction.Toolkit;

namespace com.NW84P
{
    public static class ControllerUtils
    {
        public static XRBaseController GetController(this XRBaseControllerInteractor controllerInteractor) => controllerInteractor.xrController;

        public static bool TryGetController(this IXRInteractor interactor, out XRBaseController controller)
        {
            controller = null;
            if (interactor is XRBaseControllerInteractor controllerInteractor)
            {
                controller = controllerInteractor.GetController();
            }

            return controller != null;
        }

        public static XRBaseController GetController(this IXRInteractor interactor)
        {
            XRBaseController controller = null;
            if (interactor is XRBaseControllerInteractor controllerInteractor)
            {
                controller = controllerInteractor.GetController();
            }

            return controller;
        }

        public static float GetActivateStateValue(this XRBaseController controller) => controller.activateInteractionState.value;
    }
}
