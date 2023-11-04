using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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
                _phoneBackGlass.transform.rotation
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
            Debug.Assert(_phoneBackGlass != null, "Phone Back Glass is not assigned.");
            Debug.Assert(_handwheel != null, "Handwheel is not assigned.");
            Debug.Assert(_handwheelHandles != null && _handwheelHandles.Length == 3, "Handwheel Handles are not assigned.");
            Debug.Assert(_bodyWithBackGlassPrefab != null, "Body With Back Glass Prefab is not assigned.");
            Debug.Assert(_cylinderBaseAudioSource != null, "Cylinder Base Audio Source is not assigned.");
        }

#endif
    }
}
