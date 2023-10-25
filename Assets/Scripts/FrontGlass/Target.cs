using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.NW84P
{
    public class Target : MonoBehaviour
    {
        private const float _MINIMUM_HAMMER_SPEED = 0.105f;

        private Transform _targetTransform;
        private Rigidbody _hammerRigidbody;
        private XRBaseController _controller;
        private XRGrabInteractable _hammerInteractable;
        private IXRSelectInteractor _controllerInteractor;
        private Vector3 _targetForward;

        public Action<string> OnSuccessfulHit { get; set; }

        public void OnEnable()
        {
            _targetTransform = transform;
            _targetForward = _targetTransform.forward;
        }

        public void OnTriggerEnter(Collider other)
        {
            var hammer = other.gameObject;
            if (hammer.CompareTag(Tags.Hammer) && HammerHasRigidbody(hammer))
            {
                var projectedVelocity = Vector3.Project(_hammerRigidbody.velocity, _targetForward);
                SendHapticFeedback(hammer, projectedVelocity);

                if (projectedVelocity.magnitude >= _MINIMUM_HAMMER_SPEED)
                {
                    OnSuccessfulHit?.Invoke(name);
                }
            }
        }

        private bool HammerHasRigidbody(GameObject hammer)
            => _hammerRigidbody != null || hammer.TryGetComponent(out _hammerRigidbody);

        private void SendHapticFeedback(GameObject hammer, Vector3 projectedVelocity)
        {
            if (_controller == null || IsOtherController())
            {
                var isHammerInteractableSet = _hammerInteractable != null || hammer.TryGetComponent(out _hammerInteractable);
                if (isHammerInteractableSet && _hammerInteractable.isSelected)
                {
                    _controllerInteractor = _hammerInteractable.interactorsSelecting[0];
                    _controller = _controllerInteractor.GetController();
                }
            }

            if (_controller != null && _hammerInteractable.isSelected)
            {
                var intensity = Mathf.Clamp01(projectedVelocity.magnitude);
                _controller.SendHapticImpulse(intensity, 0.2f);
            }
        }

        private bool IsOtherController()
            => _hammerInteractable.isSelected && _controllerInteractor != _hammerInteractable.interactorsSelecting[0];
    }
}
