using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.NW84P
{
    public class Handwheel : XRBaseInteractable
    {
        [Header("Handwheel Settings")]
        [SerializeField]
        private GameObject _pressCylinder;

        [SerializeField]
        private GameObject _handwheel;

        private IXRSelectInteractor _hand;
        private Quaternion _handwheelInitialRotation;
        private float _handwheelXAngleDelta;
        private Transform _handwheelTransform;
        private Rigidbody _handwheelRigidbody;

        protected override void OnEnable()
        {
            base.OnEnable();

            selectEntered.AddListener(SelectStarted);
            selectExited.AddListener(SelectExited);
            _handwheelTransform = _handwheelTransform.transform;
            _handwheelRigidbody = _handwheelTransform.GetComponent<Rigidbody>();
            _handwheelInitialRotation = _handwheelTransform.rotation;
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
                if (_hand != null)
                {
                    // track distance between hand and handwheel and apply torque to handwheel rigidbody. the force must
                    // be applied in the direction of the handwheel's forward vector and should be proportional to
                    // distance in the forward direction.
                }
                else if (_handwheelXAngleDelta <= 0)
                {
                    _handwheelTransform.rotation = _handwheelInitialRotation;
                    _handwheelXAngleDelta = 0;
                }
                else
                {
                    // should apply torque to handwheel rigidbody to rotate it back to its initial rotation.
                }
            }
            else if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                // track handwheel's X rotation delta and update the cylinder's height to match the handwheel's rotation.
            }
        }

        private void SelectStarted(SelectEnterEventArgs args)
        {
            if (args.interactorObject.transform.gameObject.CompareTag(Tags.Player))
            {
                _hand = args.interactorObject;
            }
        }

        private void SelectExited(SelectExitEventArgs args) => ResetState();

        private void ResetState()
        {
            _hand = null;
        }
    }
}
