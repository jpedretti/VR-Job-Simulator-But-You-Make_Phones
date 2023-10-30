using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

#if UNITY_EDITOR

using Unity.VisualScripting;

#endif

namespace com.NW84P
{
    [RequireComponent(typeof(CustomSocket))]
    public class FinalBase : MonoBehaviour
    {
        [SerializeField]
        private GameObject _phoneDonePrefab;

        [SerializeField]
        private Transform _playingGameObjectsParent;

        private CustomSocket _socket;
        private CustomSocket _phoneSocket;
        private FrontGlassController _glassController;

        private void Awake() => _socket = GetComponent<CustomSocket>();

        public void OnEnable() => _socket.selectEntered.AddListener(InteractableSelected);

        public void OnDisable()
        {
            _socket.selectEntered.RemoveListener(InteractableSelected);
            RemovePhoneSocketListeners();
        }

        private void InteractableSelected(SelectEnterEventArgs args)
        {
            var phone = args.interactableObject.transform.gameObject;
            RemoveInteractableScripts(phone);
            EnableCustomSocketScript(phone);
            MakeKinematic(phone);
            ConfigureCollider(phone);
        }

        private static void ConfigureCollider(GameObject phone)
        {
            if (phone.TryGetComponent<BoxCollider>(out var collider))
            {
                collider.isTrigger = true;
            }
        }

        private static void MakeKinematic(GameObject phone)
        {
            if (phone.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody.useGravity = false;
                rigidbody.isKinematic = true;
            }
        }

        private void EnableCustomSocketScript(GameObject phone)
        {
            _phoneSocket = phone.GetComponent<CustomSocket>();
            _phoneSocket.enabled = true;
            _phoneSocket.selectEntered.AddListener(FrontScreenAttached);
            _phoneSocket.selectExited.AddListener(FrontScreenDetached);
        }

        private void RemovePhoneSocketListeners()
        {
            if (_phoneSocket != null)
            {
                _phoneSocket.selectEntered.RemoveListener(FrontScreenAttached);
                _phoneSocket.selectExited.RemoveListener(FrontScreenDetached);
            }
        }

        private void FrontScreenAttached(SelectEnterEventArgs args)
        {
            _glassController = args.interactableObject.transform.gameObject.GetComponentInParent<FrontGlassController>();
            _glassController.OnGlassFixated += GlassFixed;
            _glassController.EnableTargets(enable: true);
        }

        private void GlassFixed()
        {
            _glassController.OnGlassFixated -= GlassFixed;
            RemovePhoneSocketListeners();
            var phone = _phoneSocket.transform.gameObject;
            Instantiate(_phoneDonePrefab, phone.transform.position, phone.transform.rotation, _playingGameObjectsParent);
            Destroy(phone);
            _phoneSocket = null;
        }

        private void FrontScreenDetached(SelectExitEventArgs args)
        {
            _glassController.OnGlassFixated -= GlassFixed;
            _glassController.EnableTargets(enable: false);
            _glassController = null;
        }

        private void RemoveInteractableScripts(GameObject phone)
        {
            if (phone.TryGetComponent<XRGrabInteractable>(out var interactable))
            {
                Destroy(interactable);
            }
            if (phone.TryGetComponent<XRGeneralGrabTransformer>(out var transformer))
            {
                Destroy(transformer);
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void OnValidate()
        {
            if (_phoneDonePrefab == null)
            {
                Debug.LogError("Phone Done Prefab is null");
            }

            if (_playingGameObjectsParent == null && !this.IsPrefabDefinition())
            {
                Debug.LogError("Playing Game Objects Parent is null");
            }
        }
    }
}
