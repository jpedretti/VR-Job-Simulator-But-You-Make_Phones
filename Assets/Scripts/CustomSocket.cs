using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.NW84P
{
    public class CustomSocket : XRSocketInteractor
    {
        private bool _isSnapped;
        private IXRSelectInteractor _hand;
        private XRGrabInteractable _interactable;

        private const float _SNAPPING_ALIGNMENT_THRESHOLD = 0.994f;
        private const float _UNSNAPPING_DISTANCE_THRESHOLD = 0.8f;

        protected override void OnEnable()
        {
            base.OnEnable();
            hoverSocketSnapping = false;
            hoverEntered.AddListener(HoverStarted);
            hoverExited.AddListener(HoverEnded);
            selectExited.AddListener(InteractableDeselected);
        }

        protected override void OnDisable()
        {
            hoverEntered.RemoveListener(HoverStarted);
            hoverExited.RemoveListener(HoverEnded);
            selectExited.RemoveListener(InteractableDeselected);
            base.OnDisable();
        }

        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractor(updatePhase);

            if (_interactable != null && _hand != null && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                var interactableTransform = _interactable.transform;
                StartSnap(interactableTransform);
                EndSnap(interactableTransform);
            }
        }

        private void EndSnap(Transform interactableTransform)
        {
            if (_isSnapped
                && Vector3.Distance(interactableTransform.position, _hand.transform.position) > _UNSNAPPING_DISTANCE_THRESHOLD)
            {
                EndSocketSnapping(_interactable);
                _isSnapped = false;
            }
        }

        private void StartSnap(Transform interactableTransform)
        {
            if (!_isSnapped && IsAtRightPosition(interactableTransform))
            {
                StartSocketSnapping(_interactable);
                _isSnapped = true;
            }
        }

        private bool IsAtRightPosition(Transform interactableTransform)
        {
            var interactableLocalUp = interactableTransform.InverseTransformDirection(Vector3.up);
            var attachLocalUp = attachTransform.InverseTransformDirection(Vector3.up);

            var interactableLocalForward = interactableTransform.InverseTransformDirection(Vector3.forward);
            var attachLocalForward = attachTransform.InverseTransformDirection(Vector3.forward);

            var rightUp = Vector3.Dot(interactableLocalUp, attachLocalUp) >= _SNAPPING_ALIGNMENT_THRESHOLD;
            var rightForward = Vector3.Dot(interactableLocalForward, attachLocalForward) >= _SNAPPING_ALIGNMENT_THRESHOLD;

            return rightUp && rightForward;
        }

        private void InteractableDeselected(SelectExitEventArgs args) => _isSnapped = false;

        private void HoverStarted(HoverEnterEventArgs args)
        {
            _interactable = args.interactableObject as XRGrabInteractable;

            if (_interactable != null && _interactable.interactorsSelecting.Count > 0)
            {
                _interactable.selectExited.AddListener(InteractableSelectExited);
                _hand = _interactable.interactorsSelecting[0];
            }
        }

        private void HoverEnded(HoverExitEventArgs args)
        {
            _isSnapped = false;
            Cleanup();
        }

        private void InteractableSelectExited(SelectExitEventArgs args)
        {
            if (args.interactableObject == _hand)
            {
                Cleanup();
            }
        }

        private void Cleanup()
        {
            _hand = null;

            if (_interactable != null)
            {
                _interactable.selectExited.RemoveListener(InteractableSelectExited);
                _interactable = null;
            }
        }
    }
}
