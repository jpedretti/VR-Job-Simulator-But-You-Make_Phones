using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.NW84P
{
    [Tooltip("The Torch class represents a grabbable torch object in a VR environment with a fire particle effect and a capsule collider for interaction.")]
    public class Torch : XRGrabInteractable
    {
        private const float _PARTICLE_LIFE_TIME = 0.07f;
        private const float _FIRE_COLLIDER_HEIGHT = 0.0112f;
        private const float _MIN_FIRE_SOUND_VOLUME = 0.05f;
        private const float _MAX_FIRE_SOUND_VOLUME = 1f;
        private ParticleSystem _fireParticle;
        private XRBaseController _controller;
        private ParticleSystem.MainModule _mainParticleModule;
        private CapsuleCollider _fireCollider;
        private AudioSource _fireAudioSource;
        private Vector3 _fireColliderCenter;
        private bool _isFireActive;

        public void Start()
        {
            _fireParticle = GetComponentInChildren<ParticleSystem>();
            _mainParticleModule = _fireParticle.main;
            _fireCollider = _fireParticle.GetComponent<CapsuleCollider>();
            _fireAudioSource = _fireParticle.GetComponent<AudioSource>();
            _mainParticleModule.startLifetime = 0;
            _fireCollider.enabled = false;
            _fireColliderCenter = new Vector3(_fireCollider.center.x, _fireCollider.center.y);
            _fireCollider.center = _fireColliderCenter;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            selectEntered.AddListener(SelectEntered);
            selectExited.AddListener(SelectExited);
        }

        protected override void OnDisable()
        {
            selectEntered.RemoveListener(SelectEntered);
            selectExited.RemoveListener(SelectExited);
            base.OnDisable();
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (_controller != null && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                var triggerValue = _controller.GetActivateStateValue();
                StartFire(triggerValue);
                StopFire(triggerValue);
                ControlFire(triggerValue);
            }
        }

        private void ControlFire(float triggerValue)
        {
            if (_isFireActive)
            {
                _mainParticleModule.startLifetime = triggerValue * _PARTICLE_LIFE_TIME;
                ConfigureFireCollider(triggerValue);
                _fireAudioSource.volume = Mathf.Lerp(_MIN_FIRE_SOUND_VOLUME, _MAX_FIRE_SOUND_VOLUME, triggerValue);
                _controller.SendHapticImpulse(triggerValue, 0.15f);
            }
        }

        private void StopFire(float triggerValue)
        {
            if (triggerValue == 0 && _isFireActive)
            {
                _isFireActive = false;
                _fireParticle.Stop();
                _fireAudioSource.Stop();
            }
        }

        private void StartFire(float triggerValue)
        {
            if (triggerValue > 0 && !_isFireActive)
            {
                _isFireActive = true;
                _fireParticle.Play();
                _fireAudioSource.Play();
            }
        }

        private void ConfigureFireCollider(float triggerValue)
        {
            _fireCollider.height = triggerValue * _FIRE_COLLIDER_HEIGHT;
            _fireCollider.enabled = _fireCollider.height >= _FIRE_COLLIDER_HEIGHT / 2;
            _fireColliderCenter.z = _fireCollider.height / 2;
            _fireCollider.center = _fireColliderCenter;
        }

        private void SelectEntered(SelectEnterEventArgs args) => _controller = args.interactorObject.GetController();

        private void SelectExited(SelectExitEventArgs args) => ResetState();

        private void ResetState()
        {
            _fireParticle.Stop();
            _fireAudioSource.Stop();
            _isFireActive = false;
            _mainParticleModule.startLifetime = 0;
            _controller = null;
            _fireCollider.enabled = false;
            _fireCollider.height = 0;
            _fireColliderCenter.z = 0;
            _fireCollider.center = _fireColliderCenter;
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            if (GetComponentInChildren<ParticleSystem>() == null)
            {
                Debug.LogError($"No Particle System found in any {gameObject} children");
            }

            if (GetComponentInChildren<CapsuleCollider>() == null)
            {
                Debug.LogError($"No Capsule Collider found in any {gameObject} children");
            }

            if (GetComponentInChildren<AudioSource>() == null)
            {
                Debug.LogError($"No Audio Source found in any {gameObject} children");
            }
        }
    }
}
