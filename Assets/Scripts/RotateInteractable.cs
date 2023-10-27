using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.NW84P
{
    public class RotateInteractable : XRBaseInteractable
    {
        [Header("Rotation Configuration")]
        [SerializeField]
        private RotationAxis _rotationAxis = RotationAxis.Y;

        private IXRInteractor _hand;
        private Vector3 _vector3RotationAxis;
        private Vector3 _initialHandDirection;
        private Quaternion _initialObjectRotation;

        public void Update()
        {
            if (_hand != null)
            {
                var handDirection = _hand.transform.position - transform.position;
                var projection = Vector3.ProjectOnPlane(handDirection, _vector3RotationAxis);

                var angle = Vector3.SignedAngle(_initialHandDirection, projection, _vector3RotationAxis);
                var rotation = Quaternion.AngleAxis(angle, _vector3RotationAxis);

                transform.rotation = rotation * _initialObjectRotation;

            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _vector3RotationAxis = _rotationAxis.ToVector3Axis();
            selectEntered.AddListener(SeclectStarted);
            selectExited.AddListener(SelectEnded);
        }

        protected override void OnDisable()
        {
            selectEntered.RemoveListener(SeclectStarted);
            selectExited.RemoveListener(SelectEnded);
            base.OnDisable();
        }

        private void SeclectStarted(SelectEnterEventArgs args)
        {
            _hand = args.interactorObject;

            _initialHandDirection = _hand.transform.position - transform.position;
            _initialHandDirection = Vector3.ProjectOnPlane(_initialHandDirection, _vector3RotationAxis);
            _initialObjectRotation = transform.rotation;
        }

        private void SelectEnded(SelectExitEventArgs args) => _hand = null;
    }
}
