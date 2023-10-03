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

        private const float _SCREW_MOVE_DISTANCE = 0.000015f;
        private const float _DISTANCE_TO_BE_SCREWED = 0.006F;
        private const float _DISTANCE_TO_DETACH = 0.08f;
        private const float _ALIGNEMENT_THRESHOLD = 0.85f;
        private const float _MIN_ROTATION_MULTIPLIER = 0.25f;
        private const float _MAX_ROTATION_MULTIPLIER = 1f;

        #endregion Constants

        #region Screw

        private Transform _screwTransform;
        private Vector3 _screwInitialPosition;
        private Vector3 _vector3RotationAxis;

        #endregion Screw

        #region ScrewDriver

        private Transform _screwdriverTransform;
        private Transform _screwdriverAttachTransform;
        private XRGrabInteractable _screwdriverInteractable;
        private Vector3 _screwdriverAttachDistance;
        private Vector3 _screwdirverInitialHandPosition;

        #endregion ScrewDriver

        #region Hand

        private Quaternion _handPreviousRotation;
        private Transform _handTransform;

        #endregion Hand

        #region Configurations

        private bool _isSnapped;
        private bool _isValidTriggerEnter;
        private bool _canSnap;

        #endregion Configurations

        #region Private Properties

        private bool IsScrewPositionBelowInitial
            => _rotationAxis.GetAxisValue(_screwTransform.position) < _rotationAxis.GetAxisValue(_screwInitialPosition);

        private bool IsScrewScrewed => ScrewDistance > _DISTANCE_TO_BE_SCREWED;

        private float ScrewDistance
            => _rotationAxis.GetAxisValue(_screwTransform.position) - _rotationAxis.GetAxisValue(_screwInitialPosition);

        #endregion Private Properties

        public bool IsScrewed { get; private set; }

        public UnityEvent OnScrewed = new();

        private void Start()
        {
            _screwTransform = transform;
            _vector3RotationAxis = _rotationAxis.ToVector3Axis();
            _screwInitialPosition = _screwTransform.position;
            _isValidTriggerEnter = true;
        }

        public void OnDestroy() => ResetState();

        public void OnDisable() => ResetState();

        public void OnTriggerEnter(Collider other)
        {
            if (_isValidTriggerEnter && other.gameObject.CompareTag(Tags.ScrewDriver))
            {
                _screwdriverTransform = other.gameObject.transform;
                if (InitializeXRComponents())
                {
                    _screwdirverInitialHandPosition = _handTransform.position;
                    _isValidTriggerEnter = false;
                    SetScrewdriverAttach();
                    _canSnap = true;
                }
            }
        }

        private bool InitializeXRComponents()
        {
            var result = false;
            if (_screwdriverTransform != null && _screwdriverTransform.gameObject.TryGetComponent<XRGrabInteractable>(out var interactable))
            {
                _screwdriverInteractable = interactable;
                if (interactable.interactorsSelecting.Count > 0)
                {
                    _handTransform = interactable.interactorsSelecting[0].transform;
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
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag(Tags.ScrewDriver))
            {
                ResetState();
            }
        }

        public void Update()
        {
            if (_screwdriverTransform != null && !IsScrewed)
            {
                if (!_isSnapped && _canSnap && IsCorrectAlignment())
                {
                    _isSnapped = true;
                    EnableInteractableTracking(enabled: false);
                    var sourceEuler = _screwSocketTransform.rotation.eulerAngles;
                    _screwdriverTransform.rotation = _rotationAxis.EulerAxisValue(sourceEuler, _handTransform.rotation.eulerAngles);
                    _screwdriverAttachDistance = _screwdriverAttachTransform.position - _screwdriverTransform.position;
                    _handPreviousRotation = _handTransform.rotation;
                }

                if (_isSnapped && (!_canSnap || Vector3.Distance(_screwdirverInitialHandPosition, _handTransform.position) > _DISTANCE_TO_DETACH))
                {
                    _isSnapped = false;
                    EnableInteractableTracking(enabled: true);
                }

                if (_isSnapped)
                {
                    PerformScrew();
                    VerifyScrewedState();
                }
            }
        }

        private void VerifyScrewedState()
        {
            if (IsScrewPositionBelowInitial)
            {
                _screwTransform.position = _screwInitialPosition;
            }
            else if (IsScrewScrewed)
            {
                _screwTransform.position = _screwInitialPosition + _vector3RotationAxis * _DISTANCE_TO_BE_SCREWED;
                UpdateToScrewedState();
            }
        }

        private void EnableInteractableTracking(bool enabled)
        {
            _screwdriverInteractable.trackPosition = enabled;
            _screwdriverInteractable.trackRotation = enabled;
        }

        private void PerformScrew()
        {
            var angle = GetNewAngle();

            // In this case the Z axis of the screw is in the opposite direction of the screwdriver So we need to rotate
            // the screwdriver in the opposite direction maybe it is better to make this configurable
            _screwdriverTransform.Rotate(_vector3RotationAxis, angle * -1);

            _screwTransform.Rotate(_vector3RotationAxis, angle);
            _screwTransform.position += _SCREW_MOVE_DISTANCE * angle * _vector3RotationAxis;

            _screwdriverTransform.position = _screwSocketTransform.position - _screwdriverAttachDistance;
        }

        private float GetNewAngle()
        {
            var normalizedScrewDistance = ScrewDistance / _DISTANCE_TO_BE_SCREWED;
            var rotationMultiplier = Mathf.Lerp(_MAX_ROTATION_MULTIPLIER, _MIN_ROTATION_MULTIPLIER, normalizedScrewDistance);

            //if Vector3.Dot(_screwdriverAttachTransform.forward, _handTransform.forward) >= _ALIGNEMENT_TRASHOLD;

            // Vector3 previousUp = transform.up; // Store the up vector before any changes ... (rotation changes)
            // Vector3 currentUp = transform.up; // Get the up vector after changes

            //float angleDelta = Vector3.Angle(previousUp, currentUp); // Calculate the angle
            //Vector3 cross = Vector3.Cross(previousForward, currentForward);
            //if (cross.y < 0) // Assuming y is your up axis
            //{
            //    angleDelta = -angleDelta;
            //}

            var currentRotation = _handTransform.rotation;
            var rotationDelta = currentRotation * Quaternion.Inverse(_handPreviousRotation);

            rotationDelta.ToAngleAxis(out float angle, out Vector3 axis);

            // Normalize the angle to be within the range of 0 to 360 degrees
            angle = (angle %= 360) > 180 ? angle - 360 : angle;

            if (angle != 0)
            {
                var side = Vector3.Dot(axis, _vector3RotationAxis) < 0 ? 1 : -1;
                angle = angle * rotationMultiplier * side;
            }

            _handPreviousRotation = currentRotation;
            return angle;
        }

        private bool IsCorrectAlignment()
            => Vector3.Dot(_screwdriverAttachTransform.forward, _screwSocketTransform.forward) >= _ALIGNEMENT_THRESHOLD;

        private void UpdateToScrewedState()
        {
            EnableInteractableTracking(enabled: true);
            IsScrewed = true;
            _canSnap = false;
            OnScrewed.Invoke();
        }

        private void SelectionEnded(SelectExitEventArgs args) => _canSnap = false;

        private void ActivationStarted(ActivateEventArgs args) => _canSnap = false;

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
    }
}
