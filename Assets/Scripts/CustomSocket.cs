using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.NW84P
{
    public class CustomSocket : XRSocketInteractor
    {
        [Header("Custom Configurations")]
        [Tooltip("Only allow selection of the XRGrabInteractable object if it is snapped.")]
        [SerializeField]
        private bool _onlySelectIfSnapped = false;

        private bool _isSnapped;
        private Transform _handTransform;
        private XRGrabInteractable _interactable;
        private Transform _interactableTransform;

        private const float _SNAPPING_ALIGNMENT_THRESHOLD = 0.994f;
        private const float _UNSNAPPING_DISTANCE_THRESHOLD = 0.8f;

        /// <summary>
        /// Returns true if the XRGrabInteractable object is snapped, false otherwise.
        /// </summary>
        public bool IsSnapped => _isSnapped;

        /// <summary>
        /// Gets or sets the value indicating whether only allow selection of the XRGrabInteractable object if it is snapped.
        /// </summary>
        public bool OnlySelectIfSnapped
        {
            get => _onlySelectIfSnapped;
            set => _onlySelectIfSnapped = value;
        }

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

            if (_interactable != null && _handTransform != null && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                StartSnap();
                EndSnap();
            }
        }

        public override bool CanSelect(IXRSelectInteractable interactable)
            => base.CanSelect(interactable) && (!_onlySelectIfSnapped || _isSnapped);

        private void EndSnap()
        {
            if (_isSnapped
                && Vector3.Distance(_interactableTransform.position, _handTransform.position) > _UNSNAPPING_DISTANCE_THRESHOLD)
            {
                EndSocketSnapping(_interactable);
                _isSnapped = false;
            }
        }

        private void StartSnap()
        {
            if (!_isSnapped && IsAtRightPosition())
            {
                StartSocketSnapping(_interactable);
                _isSnapped = true;
            }
        }

        private bool IsAtRightPosition()
        {
            var interactableLocalUp = _interactableTransform.up;
            var attachLocalUp = attachTransform.up;

            var interactableLocalForward = _interactableTransform.forward;
            var attachLocalForward = attachTransform.forward;

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
                _interactableTransform = _interactable.transform;
                _interactable.selectExited.AddListener(InteractableSelectExited);
                _handTransform = _interactable.interactorsSelecting[0].transform;
            }
        }

        private void HoverEnded(HoverExitEventArgs args)
        {
            _isSnapped = false;
            Cleanup();
        }

        private void InteractableSelectExited(SelectExitEventArgs args)
        {
            if (args.interactorObject.transform == _handTransform)
            {
                Cleanup();
            }
        }

        private void Cleanup()
        {
            _handTransform = null;

            if (_interactable != null)
            {
                _interactableTransform = null;
                _interactable.selectExited.RemoveListener(InteractableSelectExited);
                _interactable = null;
            }
        }
    }
}
