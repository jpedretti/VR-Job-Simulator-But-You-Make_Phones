using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.NW84P
{
    public class HandwheelHandle : XRBaseInteractable
    {
        [Header("Handwheel Handle Settings")]
        [SerializeField]
        private Handwheel _handwheel;

        private Transform _handTransform;
        private Transform _handleTransform;
        private Vector3 _selectionPoint;

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
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
            {
                if (_handTransform != null)
                {
                    var direction = _handTransform.position - _handleTransform.position;
                    var magnitude = direction.magnitude;
                    var dot = Vector3.Dot(direction.normalized, _handleTransform.forward.normalized);

                    var torque = dot * magnitude;
                    _handwheel.ApplyTorque(torque);
                }
            }
        }

        private void SelectStarted(SelectEnterEventArgs args)
        {
            if (args.interactorObject.transform.gameObject.CompareTag(Tags.Player))
            {
                _handTransform = args.interactorObject.transform;
                _selectionPoint = _handTransform.position;
            }
        }

        private void SelectExited(SelectExitEventArgs args) => ResetState();

        private void ResetState() => _handTransform = null;

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
