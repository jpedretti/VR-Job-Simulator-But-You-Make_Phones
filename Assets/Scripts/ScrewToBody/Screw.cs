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
        private const float _DELTA_ANGLE_TO_SEND_FEEDBACK = 0.32f;

        #endregion Constants

        #region Screw

        private Transform _screwTransform;
        private Vector3 _screwInitialPosition;
        private Vector3 _vector3RotationAxis;
        private AudioSource _screwAudioSource;

        #endregion Screw

        #region ScrewDriver

        private Transform _screwdriverTransform;
        private Transform _screwdriverAttachTransform;
        private XRGrabInteractable _screwdriverInteractable;
        private Vector3 _screwdriverAttachDistance;

        #endregion ScrewDriver

        #region Hand

        private Transform _handTransform;
        private XRBaseController _handController;
        private Vector3 _handInitialPosition;
        private Vector3 _handPreviousOtherTransformVector;

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

        private float ScrewDistanceNormalized => ScrewDistance / _DISTANCE_TO_BE_SCREWED;

        #endregion Private Properties

        public bool IsScrewed { get; private set; }

        public UnityEvent OnScrewed = new();

        private void Start()
        {
            _screwTransform = transform;
            _vector3RotationAxis = _rotationAxis.ToVector3Axis();
            _screwInitialPosition = _screwTransform.position;
            _isValidTriggerEnter = true;
            _screwAudioSource = GetComponent<AudioSource>();
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
                    SetHandController(interactable);
                }
                else
                {
                    _screwdriverTransform = null;
                }
            }

            return result;
        }

        private void SetHandController(XRGrabInteractable interactable)
        {
            if (interactable.interactorsSelecting[0].TryGetController(out var controller))
            {
                _handController = controller;
            }
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
                    _handPreviousOtherTransformVector = GetHandCurrentOtherTransformVector();
                    _handInitialPosition = _handTransform.position;
                }

                if (_isSnapped && (!_canSnap || Vector3.Distance(_handInitialPosition, _handTransform.position) > _DISTANCE_TO_DETACH))
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

            SendFeedback(angle);
        }

        private void SendFeedback(float angle)
        {
            if (Mathf.Abs(angle) >= _DELTA_ANGLE_TO_SEND_FEEDBACK)
            {
                _handController.SendHapticImpulse(ScrewDistanceNormalized, Time.deltaTime);
                _screwAudioSource.pitch = Mathf.Lerp(0.4f, 0.5f, ScrewDistanceNormalized);
                if (!_screwAudioSource.isPlaying)
                {
                    _screwAudioSource.Play();
                }
            }
            else
            {
                _screwAudioSource.Stop();
            }
        }

        private float GetNewAngle()
        {
            var rotationMultiplier = Mathf.Lerp(_MAX_ROTATION_MULTIPLIER, _MIN_ROTATION_MULTIPLIER, ScrewDistanceNormalized);
            var handCurrentOtherTransformVector = GetHandCurrentOtherTransformVector();
            var angleDelta = Vector3.SignedAngle(_handPreviousOtherTransformVector, handCurrentOtherTransformVector, -_vector3RotationAxis);

            if (angleDelta != 0)
            {
                angleDelta *= rotationMultiplier;
                angleDelta = FixRotationSideBasedOnScrewdriverDirection(angleDelta);
            }

            _handPreviousOtherTransformVector = handCurrentOtherTransformVector;

            return angleDelta;
        }

        private Vector3 GetHandCurrentOtherTransformVector() => _rotationAxis.OthersVector3Axis(_handTransform).Item1;

        private float FixRotationSideBasedOnScrewdriverDirection(float angleDelta)
        {
            // verify if the screwdriver rotation axis is in the same direction of the rotation axis world equivalent
            var screwdrivervector3RotationAxis = _rotationAxis.ToVector3Axis(_screwdriverTransform).normalized;
            if (Vector3.Dot(screwdrivervector3RotationAxis, _rotationAxis.ToVector3Axis()) < 0)
            {
                angleDelta = -angleDelta;
            }

            return angleDelta;
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
            _screwAudioSource.Stop();
            _isValidTriggerEnter = true;
            _isSnapped = false;
            _canSnap = false;
            _screwdriverAttachDistance = Vector3.zero;
            _screwdriverTransform = null;
            _screwdriverAttachTransform = null;
            _handController = null;
            _handTransform = null;
            if (_screwdriverInteractable != null)
            {
                EnableInteractableTracking(enabled: true);
                _screwdriverInteractable.selectExited.RemoveListener(SelectionEnded);
                _screwdriverInteractable.activated.RemoveListener(ActivationStarted);
                _screwdriverInteractable = null;
            }
        }

#if UNITY_EDITOR

        public void OnValidate()
        {
            if (_screwSocketTransform == null)
            {
                Debug.LogError($"ScrewSocketTransform is null on {gameObject.name}");
            }
            if (GetComponent<AudioSource>() == null)
            {
                Debug.LogError($"ScrewAudioSource is null on {gameObject.name}");
            }
        }

#endif
    }
}
