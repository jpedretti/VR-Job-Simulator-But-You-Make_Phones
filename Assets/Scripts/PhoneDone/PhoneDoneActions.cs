using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.NW84P
{
    [RequireComponent(typeof(XRGrabInteractable), typeof(AudioSource))]
    public class PhoneDoneActions : MonoBehaviour
    {
        [SerializeField]
        private PhoneDoneData _phoneDoneData;

        private XRGrabInteractable _phoneInteractable;
        private AudioSource _audioSource;
        private SpriteRenderer _spriteRenderer;

        public void Awake()
        {
            _phoneInteractable = GetComponent<XRGrabInteractable>();
            _audioSource = GetComponent<AudioSource>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void OnEnable() => _phoneInteractable.activated.AddListener(OnPhoneActivated);

        public void OnDisable() => _phoneInteractable.activated.RemoveListener(OnPhoneActivated);

        private void OnPhoneActivated(ActivateEventArgs arg0)
        {
            if (GameController.Instance.InsertedSinCard)
            {
                var index = UnityEngine.Random.Range(0, _phoneDoneData.Data.Length);
                var (audioClip, sprite) = _phoneDoneData.Data[index];
                _audioSource.clip = audioClip;
                _audioSource.Play();
                _spriteRenderer.sprite = sprite;
            }
        }

#if UNITY_EDITOR

        public void OnValidate() => Debug.Assert(_phoneDoneData != null, "PhoneDoneActions: PhoneDoneData is null");

#endif
    }
}
