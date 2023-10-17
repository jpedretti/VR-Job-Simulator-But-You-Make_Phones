using UnityEngine.XR.Interaction.Toolkit;

namespace com.NW84P
{
    public static class ControllerUtils
    {
        public static XRBaseController GetController(this XRBaseControllerInteractor controllerInteractor) => controllerInteractor.xrController;

        public static bool TryGetController(this IXRActivateInteractor interactor, out XRBaseController controller)
        {
            controller = null;
            if (interactor is XRBaseControllerInteractor controllerInteractor)
            {
                controller = controllerInteractor.GetController();
            }

            return controller != null;
        }

        public static bool TryGetController(this IXRSelectInteractor interactor, out XRBaseController controller)
        {
            controller = null;
            if (interactor is XRBaseControllerInteractor controllerInteractor)
            {
                controller = controllerInteractor.GetController();
            }

            return controller != null;
        }

        public static float GetActivateStateValue(this XRBaseController controller) => controller.activateInteractionState.value;

        public static float GetActivateStateValue(this IXRActivateInteractor interactable)
        {
            var result = 0f;
            if (interactable.TryGetController(out var controller))
            {
                result = controller.GetActivateStateValue();
            }

            return result;
        }

        public static void SendHapticImpulse(this IXRActivateInteractor controller, float amplitude, float duration)
        {
            if (controller.TryGetController(out var xrController))
            {
                xrController.SendHapticImpulse(amplitude, duration);
            }
        }

        public static void SendHapticImpulse(this IXRSelectInteractor controller, float amplitude, float duration)
        {
            if (controller.TryGetController(out var xrController))
            {
                xrController.SendHapticImpulse(amplitude, duration);
            }
        }
    }
}
