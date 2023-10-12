using UnityEngine;

namespace com.NW84P
{
    [RequireComponent(typeof(Rigidbody))]
    public class Handwheel : MonoBehaviour
    {
        private const float _CYLINDER_POSITION_MULTIPLIER = 0.001f;

        [SerializeField]
        private Transform _pressCylinder;

        private Transform _handwheelTransform;
        private Rigidbody _handwheelRigidbody;
        private Quaternion _handwheelInitialRotation;
        private float _handwheelXAngleDelta;
        private GameObject _roundArrows;
        private Vector3 _previousUpPosition;
        private float _torque;

        public void OnEnable()
        {
            _handwheelTransform = transform;
            _handwheelRigidbody = _handwheelTransform.GetComponent<Rigidbody>();
            _handwheelInitialRotation = _handwheelTransform.rotation;
            _previousUpPosition = _handwheelTransform.up;
            SetRoundArrows();
        }

        public void Update()
        {
            if (_handwheelRigidbody.angularVelocity.x != 0)
            {
                var angle = GetDeltaAngle();
                _handwheelXAngleDelta += angle;

                angle = HandleRotationLimits(angle);

                _pressCylinder.position += _CYLINDER_POSITION_MULTIPLIER * angle * Vector3.down;
                _previousUpPosition = _handwheelTransform.up;
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

        public void ApplyTorque(float torque) => _torque = torque;

        private float HandleRotationLimits(float angle)
        {
            if (_handwheelXAngleDelta < 0)
            {
                SetHandwheelData(0, _handwheelInitialRotation, out angle);
            }
            else if (_handwheelXAngleDelta > 142)
            {
                SetHandwheelData(142, _handwheelInitialRotation * Quaternion.Euler(142, 0, 0), out angle);
            }

            return angle;
        }

        private void SetHandwheelData(float angleDelta, Quaternion rotation, out float angle)
        {
            _handwheelTransform.rotation = rotation;
            _handwheelXAngleDelta = angleDelta;
            _handwheelRigidbody.angularVelocity = Vector3.zero;
            angle = 0;
        }

        private float GetDeltaAngle()
        {
            var angle = Vector3.Angle(_handwheelTransform.up, _previousUpPosition);
            var cross = Vector3.Cross(_handwheelTransform.up, _previousUpPosition);

            return cross.x > 0 ? angle : -angle;
        }

        private void SetRoundArrows()
        {
            for (int i = 0; i < _handwheelTransform.childCount; i++)
            {
                if (_handwheelTransform.GetChild(i).gameObject.CompareTag(Tags.RoundArrows))
                {
                    _roundArrows = _handwheelTransform.GetChild(i).gameObject;
                    break;
                }
            }
        }

#if UNITY_EDITOR

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
        }

#endif
    }
}
