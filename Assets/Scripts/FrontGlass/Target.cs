using System;
using UnityEngine;

namespace com.NW84P
{
    public class Target : MonoBehaviour
    {
        private const float _MINIMUM_HAMMER_SPEED = 0.5f;

        private Transform _targetTransform;
        private Rigidbody _hammerRigidbody;

        public Action<string> OnSuccessfulHit { get; set; }

        public void OnEnable() => _targetTransform = transform;

        public void OnDisable() => _hammerRigidbody = null;

        public void OnTriggerEnter(Collider other)
        {
            var hammer = other.gameObject;
            if (hammer.CompareTag(Tags.Hammer))
            {
                if(_hammerRigidbody == null)
                {
                    hammer.TryGetComponent(out _hammerRigidbody);
                }

                var projectedVelocity = Vector3.Project(_hammerRigidbody.velocity, _targetTransform.forward);
                if (projectedVelocity.magnitude >= _MINIMUM_HAMMER_SPEED)
                {
                    OnSuccessfulHit?.Invoke(name);
                }
            }
        }
    }
}
