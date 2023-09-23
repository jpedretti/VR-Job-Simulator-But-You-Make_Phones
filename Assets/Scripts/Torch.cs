using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.NW84P
{
    public class Torch : XRGrabInteractable
    {
        private ParticleSystem _fireParticle;
        private XRBaseController _controller;
        private ParticleSystem.MainModule _mainParticlaModule;

        public void Start()
        {
            _fireParticle = GetComponentInChildren<ParticleSystem>();
            _mainParticlaModule = _fireParticle.main;
            _mainParticlaModule.startLifetime = 0;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            activated.AddListener(Activate);
            deactivated.AddListener(Deactivate);
        }

        protected override void OnDisable()
        {
            activated.RemoveListener(Activate);
            deactivated.RemoveListener(Deactivate);
            base.OnDisable();
        }

        protected override void ProcessInteractionStrength(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractionStrength(updatePhase);

            if (_controller != null && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                var triggerValue = _controller.activateInteractionState.value;
                _mainParticlaModule.startLifetime = triggerValue * 0.2f;
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

        private void Deactivate(DeactivateEventArgs args)
        {
            _fireParticle.Stop();
            _mainParticlaModule.startLifetime = 0;
            _controller = null;
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (GetComponentInChildren<ParticleSystem>() == null)
            {
                Debug.LogError($"No Particle System found in {gameObject}");
            }
        }

#endif
    }
}
