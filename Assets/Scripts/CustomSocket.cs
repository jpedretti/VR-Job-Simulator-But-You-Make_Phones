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

        [SerializeField]
        [Tooltip("The threshold for the alignment of the interactable object and the socket.")]
        private float _snapAlignmentThreshold = 0.92f;

        [SerializeField]
        [Tooltip("the distance between the hand and the interactable so that the interactable will be unsnapped from the socket")]
        private float _unsnapDistance = 0.8f;

        [SerializeField]
        [Tooltip("Should play haptic feedback when the interactable is snapped.")]
        private bool _snapHapticFeedback;

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("The amplitude of the haptic feedback when the interactable is snapped.")]
        private float _snapHapticFeedbackAmplitude = 1;

        [SerializeField]
        [Range(0, float.PositiveInfinity)]
        [Tooltip("The duration of the haptic feedback when the interactable is snapped.")]
        private float _snapHapticFeedbackDuration = 0.1f;

        private bool _isSnapped;
        private Transform _handTransform;
        private XRBaseController _handController;
        private XRGrabInteractable _interactable;
        private Transform _interactableTransform;

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
                && Vector3.Distance(_interactableTransform.position, _handTransform.position) > _unsnapDistance)
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
                SendHapticFeedback();
            }
        }

        private void SendHapticFeedback()
        {
            if (_handController != null && _snapHapticFeedback)
            {
                _handController.SendHapticImpulse(_snapHapticFeedbackAmplitude, _snapHapticFeedbackDuration);
            }
        }

        private bool IsAtRightPosition()
        {
            var interactableLocalUp = _interactableTransform.up;
            var attachLocalUp = attachTransform.up;

            var interactableLocalForward = _interactableTransform.forward;
            var attachLocalForward = attachTransform.forward;

            var rightUp = Vector3.Dot(interactableLocalUp, attachLocalUp) >= _snapAlignmentThreshold;
            var rightForward = Vector3.Dot(interactableLocalForward, attachLocalForward) >= _snapAlignmentThreshold;

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
                _handController = _interactable.interactorsSelecting[0].GetController();
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
