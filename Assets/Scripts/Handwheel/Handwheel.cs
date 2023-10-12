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
        private Vector3 _previousUpRotation;

        public void OnEnable()
        {
            _handwheelTransform = transform;
            _handwheelRigidbody = _handwheelTransform.GetComponent<Rigidbody>();
            _handwheelInitialRotation = _handwheelTransform.rotation;
            _previousUpRotation = _handwheelTransform.up;

            SetRoundArrows();
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

        public void Update()
        {
            if (_handwheelRigidbody.angularVelocity.x != 0)
            {
                var angle = GetDeltaAngle();
                _handwheelXAngleDelta += angle;

                if (_handwheelXAngleDelta < 0)
                {
                    _handwheelTransform.rotation = _handwheelInitialRotation;
                    _handwheelXAngleDelta = 0;
                    _handwheelRigidbody.angularVelocity = Vector3.zero;
                    angle = 0;
                }


                _pressCylinder.position += _CYLINDER_POSITION_MULTIPLIER * angle * Vector3.down;
                _previousUpRotation = _handwheelTransform.up;

                Debug.Log($"_handwheelXAngleDelta: {_handwheelXAngleDelta}");
                Debug.Log("=====================================");
            }
        }

        public void ApplyTorque(float torque)
        {
            _handwheelRigidbody.AddRelativeTorque(torque, 0, 0);
        }

        private float GetDeltaAngle()
        {
            var angle = Vector3.Angle(_handwheelTransform.up, _previousUpRotation);
            var cross = Vector3.Cross(_handwheelTransform.up, _previousUpRotation);

            return cross.x > 0 ? angle : -angle;
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
