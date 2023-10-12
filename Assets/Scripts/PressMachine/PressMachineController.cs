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

        private GameObject _oldPhoneBody;

        public void OnEnable()
        {
            _phoneBackGlass.selectEntered.AddListener(BodyAttachedToGlass);
            _phoneBackGlass.selectExited.AddListener(BodyDetachedFromGlass);
            _handwheel.OnFinishedPressing += HandlePressFinished;
        }

        public void OnDisable() => ClearState();

        private void ClearState()
        {
            _phoneBackGlass.selectEntered.RemoveListener(BodyAttachedToGlass);
            _phoneBackGlass.selectExited.RemoveListener(BodyDetachedFromGlass);
            _handwheel.OnFinishedPressing -= HandlePressFinished;
        }

        private void HandlePressFinished()
        {
            Instantiate(_bodyWithBackGlassPrefab, _oldPhoneBody.transform.position, _phoneBackGlass.transform.rotation);
            ClearState();
            Destroy(_phoneBackGlass.gameObject);
            Destroy(_oldPhoneBody);
        }

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
        }

#endif
    }
}
