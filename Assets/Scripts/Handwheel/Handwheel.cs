using UnityEngine;

namespace com.NW84P
{
    [RequireComponent(typeof(Rigidbody))]
    public class Handwheel : MonoBehaviour
    {

        [SerializeField]
        private Transform _pressCylinder;

        private Transform _handwheelTransform;
        private Rigidbody _handwheelRigidbody;
        private Quaternion _handwheelInitialRotation;
        private float _handwheelXAngleDelta;
        private  float _handwheelPreviousXAngle;
        private GameObject _roundArrows;

        public void OnEnable()
        {
            _handwheelTransform = transform;
            _handwheelRigidbody = _handwheelTransform.GetComponent<Rigidbody>();
            _handwheelInitialRotation = _handwheelTransform.rotation;

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
            // track handwheel's X rotation delta and update the cylinder's height to match the handwheel's rotation.

            if (_handwheelXAngleDelta < 0)
            {
                _handwheelTransform.rotation = _handwheelInitialRotation;
                _handwheelXAngleDelta = 0;
            }
        }

        public void ApplyTorque(float torque)
        {
            // should apply torque to handwheel rigidbody to rotate it back to its initial rotation.
            _handwheelRigidbody.AddRelativeTorque(torque, 0, 0);
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
