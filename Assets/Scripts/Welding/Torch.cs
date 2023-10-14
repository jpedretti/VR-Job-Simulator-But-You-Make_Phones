using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.NW84P
{
    [Tooltip("The Torch class represents a grabbable torch object in a VR environment with a fire particle effect and a capsule collider for interaction.")]
    public class Torch : XRGrabInteractable
    {
        private const float _ParticleLifeTime = 0.15f;
        private const float _FireColliderHeight = 0.017f;

        private ParticleSystem _fireParticle;
        private XRBaseController _controller;
        private ParticleSystem.MainModule _mainParticlaModule;
        private CapsuleCollider _fireCollider;
        private Vector3 _fireColliderCenter;

        public void Start()
        {
            _fireParticle = GetComponentInChildren<ParticleSystem>();
            _mainParticlaModule = _fireParticle.main;
            _fireCollider = _fireParticle.GetComponent<CapsuleCollider>();
            _mainParticlaModule.startLifetime = 0;
            _fireCollider.enabled = false;
            _fireColliderCenter = new Vector3(_fireCollider.center.x, _fireCollider.center.y, 0);
            _fireCollider.center = _fireColliderCenter;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            activated.AddListener(Activate);
            deactivated.AddListener(Deactivate);
            selectExited.AddListener(SelectExited);
        }

        protected override void OnDisable()
        {
            activated.RemoveListener(Activate);
            deactivated.RemoveListener(Deactivate);
            selectExited.RemoveListener(SelectExited);
            base.OnDisable();
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (_controller != null && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                var triggerValue = _controller.activateInteractionState.value;
                _mainParticlaModule.startLifetime = triggerValue * _ParticleLifeTime;
                _fireCollider.height = triggerValue * _FireColliderHeight;
                _fireCollider.enabled = _fireCollider.height >= _FireColliderHeight / 2;
                _fireColliderCenter.z = _fireCollider.height;
                _fireCollider.center = _fireColliderCenter;
            }
        }

        private void Activate(ActivateEventArgs args)
        {
            if (args.interactorObject is XRBaseControllerInteractor controllerInteractor)
            {
                _controller = controllerInteractor.xrController;
                _fireParticle.Play();
            }
        }

        private void Deactivate(DeactivateEventArgs args) => ResetState();

        private void SelectExited(SelectExitEventArgs args) => ResetState();

        private void ResetState()
        {
            _fireParticle.Stop();
            _mainParticlaModule.startLifetime = 0;
            _controller = null;
            _fireCollider.enabled = false;
            _fireCollider.height = 0;
            _fireColliderCenter.z = 0;
            _fireCollider.center = _fireColliderCenter;
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (GetComponentInChildren<ParticleSystem>() == null)
            {
                Debug.LogError($"No Particle System found in {gameObject}");
            }

            if (GetComponentInChildren<CapsuleCollider>() == null)
            {
                Debug.LogError($"No Capsule Collider found in {gameObject}");
            }
        }

#endif
    }
}
