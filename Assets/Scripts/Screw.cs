using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.NW84P
{
    public class Screw : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private RotationAxis _rotationAxis = RotationAxis.Y;

        [SerializeField]
        private Transform _screwSocketTransform;

        #endregion Serialized Fields

        #region Constants

        private const float _SCREW_MOVE_DISTANCE = 0.00001f;
        private const float _SCREW_DISTANCE = 0.006F;

        #endregion Constants

        #region Screw

        private Transform _screwTransform;
        private Vector3 _screwInitialPosition;
        private Vector3 _vector3RotationAxis;

        #endregion Screw

        #region ScrewDriver

        private Transform _screwdriverTransform;
        private Transform _screwdriverAttachTransform;
        private Quaternion _screwdriverPreviousRotation;
        private XRGrabInteractable _screwdriverInteractable;
        private Vector3 _screwdriverAttachDistance;

        #endregion ScrewDriver

        #region Hand

        private float _handOriginalRotation;
        private Transform _handTransform;

        #endregion Hand

        #region Configurations

        private float _rotationMultiplier = 1f;
        private bool _isSnapped;
        private bool _isValidTriggerEnter;
        private bool _canSnap;

        #endregion Configurations

        #region Private Properties

        private bool IsScrewPositionBelowInitial
            => _rotationAxis.GetAxisValue(_screwTransform.position) < _rotationAxis.GetAxisValue(_screwInitialPosition);

        private bool IsScrewScrewed
            => _rotationAxis.GetAxisValue(_screwTransform.position) - _rotationAxis.GetAxisValue(_screwInitialPosition)
                > _SCREW_DISTANCE;

        #endregion Private Properties

        public bool IsScrewed { get; private set; }

        public UnityEvent OnScrewed = new();

        private void Start()
        {
            _screwTransform = transform.parent;
            _vector3RotationAxis = _rotationAxis.ToVector3Axis();
            _screwInitialPosition = _screwTransform.position;
            _isValidTriggerEnter = true;
        }

        public void OnDestroy() => ResetState();

        public void OnDisable() => ResetState();

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(Tags.ScrewDriver) && _isValidTriggerEnter)
            {
                _screwdriverTransform = other.gameObject.transform;
                if (InitializeXRComponents())
                {
                    _canSnap = true;
                    _isValidTriggerEnter = false;
                    _screwdriverPreviousRotation = _screwdriverTransform.rotation;
                    SetScrewdriverAttach();
                }
            }
        }

        private bool InitializeXRComponents()
        {
            var result = false;
            if (_screwdriverTransform.gameObject.TryGetComponent<XRGrabInteractable>(out var interactable))
            {
                _screwdriverInteractable = interactable;
                if (interactable.interactorsSelecting.Count > 0)
                {
                    _handTransform = interactable.interactorsSelecting[0].transform;
                    _handOriginalRotation = _handTransform.rotation.eulerAngles.z;
                    interactable.selectExited.AddListener(SelectionEnded);
                    interactable.activated.AddListener(ActivationStarted);
                    result = true;
                }
                else
                {
                    _screwdriverTransform = null;
                }
            }

            return result;
        }

        private void SetScrewdriverAttach()
        {
            for (var i = 0; i < _screwdriverTransform.childCount; i++)
            {
                var child = _screwdriverTransform.GetChild(i);
                if (child.CompareTag(Tags.ScrewDriverAttach))
                {
                    _screwdriverAttachTransform = child;
                    break;
                }
            }

            _screwdriverAttachDistance = new(0, 0, _screwdriverAttachTransform.position.z - _screwdriverTransform.position.z);
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag(Tags.ScrewDriver))
            {
                ResetState();
            }
        }

        private void ResetState()
        {
            _isValidTriggerEnter = true;
            _isSnapped = false;
            _canSnap = false;
            _screwdriverAttachDistance = Vector3.zero;
            _screwdriverTransform = null;
            _screwdriverAttachTransform = null;
            if (_screwdriverInteractable != null)
            {
                EnableInteractableTracking(enabled: true);
                _screwdriverInteractable.selectExited.RemoveListener(SelectionEnded);
                _screwdriverInteractable.activated.RemoveListener(ActivationStarted);
                _screwdriverInteractable = null;
            }
        }

        public void Update()
        {
            if (_screwdriverTransform != null && !IsScrewed)
            {
                if (!_isSnapped && _canSnap && IsAtRightPosition())
                {
                    _isSnapped = true;
                    EnableInteractableTracking(enabled: false);
                    RotateScrewdriverWithHand();
                    _screwdriverPreviousRotation = _screwdriverTransform.rotation;
                }

                if (_isSnapped && (!_canSnap || Vector3.Distance(_screwdriverTransform.position, _handTransform.position) > 0.15f))
                {
                    _isSnapped = false;
                    EnableInteractableTracking(enabled: true);
                }

                if (_isSnapped)
                {
                    RotateScrewdriverWithHand();
                    PerformScrew();
                    _screwdriverTransform.position = _screwSocketTransform.position - _screwdriverAttachDistance;
                }
            }
        }

        private void EnableInteractableTracking(bool enabled)
        {
            _screwdriverInteractable.trackPosition = enabled;
            _screwdriverInteractable.trackRotation = enabled;
        }

        private void RotateScrewdriverWithHand()
        {
            var sourceEuler = _screwSocketTransform.rotation.eulerAngles;
            var originalRotationAxisRotation = _handOriginalRotation + _handTransform.rotation.eulerAngles.z;
            var newRotation = Quaternion.Euler(sourceEuler.x, sourceEuler.y, originalRotationAxisRotation);
            _screwdriverTransform.rotation = newRotation;
        }

        private bool IsAtRightPosition()
        {
            var interactableLocalUp = _screwdriverAttachTransform.InverseTransformDirection(_vector3RotationAxis);
            var attachLocalUp = _screwSocketTransform.InverseTransformDirection(_vector3RotationAxis);
            return Vector3.Dot(interactableLocalUp, attachLocalUp) >= 0.85f;
        }

        private void PerformScrew()
        {
            if (!IsScrewPositionBelowInitial && !IsScrewScrewed)
            {
                var currentRotation = _screwdriverTransform.rotation;
                var rotationDelta = currentRotation * Quaternion.Inverse(_screwdriverPreviousRotation);

                rotationDelta.ToAngleAxis(out float angle, out Vector3 axis);

                // Normalize the angle to be within the range of 0 to 360 degrees
                angle = (angle %= 360) > 180 ? angle - 360 : angle;

                var side = Vector3.Dot(axis, _vector3RotationAxis) < 0 ? 1 : -1;

                float screwRotation = angle * _rotationMultiplier * side;
                _screwTransform.Rotate(_vector3RotationAxis, screwRotation);

                float moveDistance = screwRotation * _SCREW_MOVE_DISTANCE;
                _screwTransform.position += _vector3RotationAxis * moveDistance;

                _screwdriverPreviousRotation = currentRotation;
            }

            if (IsScrewPositionBelowInitial)
            {
                _screwTransform.position = _screwInitialPosition;
            }
            else if (IsScrewScrewed)
            {
                _screwTransform.position = _screwInitialPosition + _vector3RotationAxis * _SCREW_DISTANCE;
                UpdateToScrewedState();
            }
        }

        private void UpdateToScrewedState()
        {
            EnableInteractableTracking(enabled: true);
            IsScrewed = true;
            _canSnap = false;
            OnScrewed.Invoke();
        }

        private void SelectionEnded(SelectExitEventArgs args) => _canSnap = false;

        private void ActivationStarted(ActivateEventArgs args) => _canSnap = false;
    }
}
