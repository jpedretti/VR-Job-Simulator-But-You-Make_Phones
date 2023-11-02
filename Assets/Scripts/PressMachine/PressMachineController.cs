using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

#if UNITY_EDITOR

using Unity.VisualScripting;

#endif

namespace com.NW84P
{
    public class PressMachineController : MonoBehaviour
    {
        [SerializeField]
        private CustomSocket _phoneBackGlass;

        [SerializeField]
        private Handwheel _handwheel;

        [SerializeField]
        private HandwheelHandle[] _handwheelHandles;

        [SerializeField]
        private GameObject _bodyWithBackGlassPrefab;

        [SerializeField]
        private AudioSource _cylinderBaseAudioSource;

        [SerializeField]
        private Transform _playingGameObjectsParent;

        private GameObject _oldPhoneBody;
        private XRGrabInteractable _newPhoneGrabInteractable;

        public void OnEnable()
        {
            _phoneBackGlass.selectEntered.AddListener(BodyAttachedToGlass);
            _phoneBackGlass.selectExited.AddListener(BodyDetachedFromGlass);
            _handwheel.OnFinishedPressing += HandlePressFinished;
        }

        public void OnDisable()
        {
            RemoveBeforePressListeners();
            EnableHandwheel(false);
            if (_newPhoneGrabInteractable != null)
            {
                _newPhoneGrabInteractable.selectEntered.RemoveListener(NewPhoneSelected);
                _newPhoneGrabInteractable = null;
            }
        }

        private void RemoveBeforePressListeners()
        {
            _phoneBackGlass.selectEntered.RemoveListener(BodyAttachedToGlass);
            _phoneBackGlass.selectExited.RemoveListener(BodyDetachedFromGlass);
            _handwheel.OnFinishedPressing -= HandlePressFinished;
        }

        private void HandlePressFinished()
        {
            _cylinderBaseAudioSource.Play();
            CreateNewPhone();
            RemoveBeforePressListeners();
            Destroy(_phoneBackGlass.gameObject);
            Destroy(_oldPhoneBody);
            _oldPhoneBody = null;
        }

        private void CreateNewPhone()
        {
            var newPhone = Instantiate(
                _bodyWithBackGlassPrefab,
                _oldPhoneBody.transform.position,
                _phoneBackGlass.transform.rotation,
                _playingGameObjectsParent
            );
            _newPhoneGrabInteractable = newPhone.GetComponent<XRGrabInteractable>();
            _newPhoneGrabInteractable.selectEntered.AddListener(NewPhoneSelected);
        }

        private void NewPhoneSelected(SelectEnterEventArgs _) => enabled = false;

        private void BodyAttachedToGlass(SelectEnterEventArgs args)
        {
            _oldPhoneBody = args.interactableObject.transform.gameObject;
            EnableHandwheel(true);
        }

        private void BodyDetachedFromGlass(SelectExitEventArgs _)
        {
            _oldPhoneBody = null;
            EnableHandwheel(false);
        }

        private void EnableHandwheel(bool enabled)
        {
            _handwheel.enabled = enabled;
            for (int i = 0; i < _handwheelHandles.Length; i++)
            {
                _handwheelHandles[i].enabled = enabled;
                _handwheelHandles[i].gameObject.GetComponent<Collider>().enabled = enabled;
            }
        }

#if UNITY_EDITOR

        public void OnValidate()
        {
            if (_phoneBackGlass == null)
            {
                Debug.LogError("Phone Back Glass is not assigned.");
            }

            if (_handwheel == null)
            {
                Debug.LogError("Handwheel is not assigned.");
            }

            if (_handwheelHandles == null || _handwheelHandles.Length != 3)
            {
                Debug.LogError("Handwheel Handles are not assigned.");
            }

            if (_bodyWithBackGlassPrefab == null)
            {
                Debug.LogError("Body With Back Glass Prefab is not assigned.");
            }

            if (_cylinderBaseAudioSource == null)
            {
                Debug.LogError("Cylinder Base Audio Source is not assigned.");
            }

            if (_playingGameObjectsParent == null && !this.IsPrefabDefinition())
            {
                Debug.LogError($"PlayingGameObjectsParent is null on {gameObject.name}");
            }
        }

#endif
    }
}
