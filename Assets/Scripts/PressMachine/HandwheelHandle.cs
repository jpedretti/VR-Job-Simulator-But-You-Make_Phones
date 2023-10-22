using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.NW84P
{
    public class HandwheelHandle : XRBaseInteractable
    {
        private const float _MAX_TORQUE = 0.18f;

        [Header("Handwheel Handle Settings")]
        [SerializeField]
        private Handwheel _handwheel;

        private Transform _handTransform;
        private Transform _handleTransform;
        private XRBaseController _handController;
        private Vector3 _selectionPoint;
        private Vector3 _previousHandlePosition;

        protected override void OnEnable()
        {
            base.OnEnable();

            _handleTransform = transform;
            selectEntered.AddListener(SelectStarted);
            selectExited.AddListener(SelectExited);
        }

        protected override void OnDisable()
        {
            ResetState();
            selectEntered.RemoveListener(SelectStarted);
            selectExited.RemoveListener(SelectExited);
            base.OnDisable();
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && _handTransform != null)
            {
                var torque = CalculateTorque();
                var absTorque = Mathf.Abs(torque);
                if (absTorque <= _MAX_TORQUE)
                {
                    _handwheel.ApplyTorque(torque);
                    SendHapticFeedback(absTorque);
                    _selectionPoint += _handleTransform.position - _previousHandlePosition;
                    _previousHandlePosition = _handleTransform.position;
                }
                else
                {
                    ResetState();
                }
            }
        }

        private float CalculateTorque()
        {
            var direction = _handTransform.position - _selectionPoint;
            var dot = Vector3.Dot(direction.normalized, _handleTransform.forward.normalized);
            return dot * direction.magnitude;
        }

        private void SendHapticFeedback(float absTorque)
        {
            if (_handController != null)
            {
                var amplitude = absTorque / _MAX_TORQUE;
                _handController.SendHapticImpulse(amplitude, 0.1f);
            }
        }

        private void SelectStarted(SelectEnterEventArgs args)
        {
            var handTransform = args.interactorObject.transform;
            if (handTransform.gameObject.CompareTag(Tags.Player))
            {
                _handwheel.IsHolding = true;
                _handTransform = handTransform;
                _handController = args.interactorObject.GetController();
                _selectionPoint = _handTransform.position;
                _previousHandlePosition = _handleTransform.position;
            }
        }

        private void SelectExited(SelectExitEventArgs args) => ResetState();

        private void ResetState()
        {
            _handwheel.IsHolding = false;
            _handwheel.ApplyTorque(0);
            _handTransform = null;
            _handController = null;
        }

#if UNITY_EDITOR

        public void OnValidate()
        {
            if (_handwheel == null)
            {
                Debug.LogError($"Handwheel is not set in {gameObject}.");
            }
        }

#endif
    }
}
