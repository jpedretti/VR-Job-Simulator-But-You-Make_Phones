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

        public void OnEnable()
        {
            _phoneBackGlass.selectEntered.AddListener(BodyAttachedToGlass);
            _phoneBackGlass.selectExited.AddListener(BodyDetachedFromGlass);
        }

        public void OnDisable()
        {
            _phoneBackGlass.selectEntered.RemoveListener(BodyAttachedToGlass);
            _phoneBackGlass.selectExited.RemoveListener(BodyDetachedFromGlass);
        }

        private void BodyAttachedToGlass(SelectEnterEventArgs _) => EnableHandwheel(true);

        private void BodyDetachedFromGlass(SelectExitEventArgs _) => EnableHandwheel(false);

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

            if (_handwheelHandles == null || _handwheelHandles.Length == 0)
            {
                Debug.LogError("Handwheel Handles are not assigned.");
            }
        }
#endif
    }
}
