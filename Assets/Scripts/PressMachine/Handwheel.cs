using System;
using UnityEngine;

namespace com.NW84P
{
    [RequireComponent(typeof(Rigidbody))]
    public class Handwheel : MonoBehaviour
    {
        private const float _ANGLE_CYLINDER_POSITION_RELATION = 1.5f;
        private const float _CYLINDER_POSITION_MULTIPLIER = 0.001f * _ANGLE_CYLINDER_POSITION_RELATION;
        private const float _MAX_X_ANGLE = 140 / _ANGLE_CYLINDER_POSITION_RELATION;
        private const float _MAX_ANGULAR_VELOCITY_MAGNITUDE = 0.3f;

        [SerializeField]
        private Transform _pressCylinder;

        private Transform _handwheelTransform;
        private Rigidbody _handwheelRigidbody;
        private AudioSource _handwheelAudioSource;
        private Quaternion _handwheelInitialRotation;
        private Quaternion _handwheelFinalRotation;
        private float _handwheelXAngleDelta;
        private GameObject _roundArrows;
        private Vector3 _previousUpPosition;
        private float _torque;
        private bool _isInLimitPosition;
        private byte _holdingsCount;

        public Action OnFinishedPressing { get; set; }
        public bool IsPressed { get; set; }

        public bool IsHolding
        {
            get => _holdingsCount > 0;
            set { if (value) _holdingsCount++; else _holdingsCount--; }
        }

        public void Awake()
        {
            _handwheelTransform = transform;
            _handwheelRigidbody = GetComponent<Rigidbody>();
            _handwheelInitialRotation = _handwheelTransform.rotation;
            _previousUpPosition = _handwheelTransform.up;
            _handwheelAudioSource = GetComponent<AudioSource>();
            _handwheelFinalRotation = _handwheelInitialRotation * Quaternion.Euler(_MAX_X_ANGLE, 0, 0);
            SetRoundArrows();
        }

        public void OnEnable() => _roundArrows.SetActive(true);

        public void OnDisable() => _roundArrows.SetActive(false);

        public void Update()
        {
            if (IsHolding)
            {
                var angularVelocity = _handwheelRigidbody.angularVelocity;
                var clampedAngularVelocityX = Mathf.Clamp(angularVelocity.x, -_MAX_ANGULAR_VELOCITY_MAGNITUDE, _MAX_ANGULAR_VELOCITY_MAGNITUDE);
                MoveCylinder(angularVelocity, clampedAngularVelocityX);
                var playSound = !_isInLimitPosition && Mathf.Abs(angularVelocity.x) > 0.005f;
                PlaySound(playSound, clampedAngularVelocityX);
            }
        }

        private void MoveCylinder(Vector3 angularVelocity, float clampedAngularVelocityX)
        {
            if (angularVelocity.x != 0)
            {
                var angle = GetDeltaAngle();
                _handwheelXAngleDelta += angle;
                angle = HandleRotationLimits(angle);

                _pressCylinder.position += _CYLINDER_POSITION_MULTIPLIER * angle * Vector3.down;
                _previousUpPosition = _handwheelTransform.up;

                _handwheelRigidbody.angularVelocity = new(clampedAngularVelocityX, angularVelocity.y, angularVelocity.z);
            }
        }

        public void FixedUpdate()
        {
            if (_torque != 0)
            {
                _handwheelRigidbody.AddRelativeTorque(_torque, 0, 0);
                _torque = 0;
            }
        }

        public void ApplyTorque(float torque)
        {
            _torque = torque;
            if (torque == 0)
            {
                _handwheelRigidbody.angularVelocity = Vector3.zero;
                PlaySound(false);
            }
        }

        private float HandleRotationLimits(float angle)
        {
            _isInLimitPosition = false;
            if (_handwheelXAngleDelta < 0)
            {
                SetHandwheelData(0, _handwheelInitialRotation, out angle);
                _isInLimitPosition = true;
            }
            else if (_handwheelXAngleDelta > _MAX_X_ANGLE)
            {
                _isInLimitPosition = true;
                SetHandwheelData(_MAX_X_ANGLE, _handwheelFinalRotation, out angle);
                if (!IsPressed)
                {
                    OnFinishedPressing?.Invoke();
                    IsPressed = true;
                    _roundArrows.transform.Rotate(0, 0, 180);
                }
            }

            return angle;
        }

        private void PlaySound(bool shouldPlay, float clampedAngularVelocityX = 0)
        {
            if (shouldPlay && !_handwheelAudioSource.isPlaying)
            {
                var normalizedVelocity = Mathf.Abs(clampedAngularVelocityX) / _MAX_ANGULAR_VELOCITY_MAGNITUDE;
                _handwheelAudioSource.pitch = Mathf.Lerp(0.9f, 0.97f, normalizedVelocity);
                _handwheelAudioSource.Play();
            }
            else if (!shouldPlay && _handwheelAudioSource.isPlaying)
            {
                _handwheelAudioSource.Stop();
            }
        }

        private void SetHandwheelData(float angleDelta, Quaternion rotation, out float angle)
        {
            _handwheelTransform.rotation = rotation;
            _handwheelXAngleDelta = angleDelta;
            _handwheelRigidbody.angularVelocity = Vector3.zero;
            angle = 0;
        }

        private float GetDeltaAngle()
            => Vector3.SignedAngle(_handwheelTransform.up, _previousUpPosition, -_handwheelTransform.right);

        private void SetRoundArrows()
        {
            for (int i = 0; i < _handwheelTransform.childCount; i++)
            {
                var child = _handwheelTransform.GetChild(i);
                if (child.CompareTag(Tags.RoundArrows))
                {
                    _roundArrows = child.gameObject;
                    break;
                }
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void OnValidate()
        {
            if (_pressCylinder == null)
            {
                Debug.LogError($"Press Cylinder is not set in {gameObject}.");
            }

            _handwheelTransform = transform;
            SetRoundArrows();
            if (_roundArrows == null)
            {
                Debug.LogError($"Round Arrows is not set in {gameObject}.");
            }

            if (GetComponent<AudioSource>() == null)
            {
                Debug.LogError($"Audio Source is not set in {gameObject}.");
            }
        }
    }
}
