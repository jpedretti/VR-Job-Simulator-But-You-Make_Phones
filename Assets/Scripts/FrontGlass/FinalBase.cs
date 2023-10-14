using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

namespace com.NW84P
{
    [RequireComponent(typeof(CustomSocket))]
    public class FinalBase : MonoBehaviour
    {
        [SerializeField]
        private GameObject _phoneDonePrefab;

        private CustomSocket _socket;
        private CustomSocket _phoneSocket;
        private GameObject _phone;
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
            _phone = args.interactableObject.transform.gameObject;
            RemoveInteractableScripts();
            AddCustomSocketScript();
            MakeKinematic();
            ConfigureCollider();
        }

        private void ConfigureCollider()
        {
            if (_phone.TryGetComponent<BoxCollider>(out var collider))
            {
                collider.isTrigger = true;
            }
        }

        private void MakeKinematic()
        {
            if (_phone.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody.useGravity = false;
                rigidbody.isKinematic = true;
            }
        }

        private void AddCustomSocketScript()
        {
            _phoneSocket = _phone.AddComponent<CustomSocket>();
            _phoneSocket.hoverSocketSnapping = false;
            _phoneSocket.OnlySelectIfSnapped = true;
            _phoneSocket.showInteractableHoverMeshes = false;
            _phoneSocket.interactionLayers = LayerMasks.FRONT_GLASS;
            for (int i = 0; i < _phone.transform.childCount; i++)
            {
                var child = _phone.transform.GetChild(i);
                if (child.gameObject.CompareTag(Tags.Socket))
                {
                    _phoneSocket.attachTransform = child;
                    break;
                }
            }
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
            _phoneSocket = null;
            Instantiate(_phoneDonePrefab, _phone.transform.position, _phone.transform.rotation);
            Destroy(_phone);
        }

        private void FrontScreenDetached(SelectExitEventArgs args)
        {
            _glassController.OnGlassFixated -= GlassFixed;
            _glassController.EnableTargets(enable: false);
            _glassController = null;
        }

        private void RemoveInteractableScripts()
        {
            if (_phone.TryGetComponent<XRGrabInteractable>(out var interactable))
            {
                Destroy(interactable);
            }
            if (_phone.TryGetComponent<XRGeneralGrabTransformer>(out var transformer))
            {
                Destroy(transformer);
            }
        }

#if UNITY_EDITOR

        public void OnValidate()
        {
            if (_phoneDonePrefab == null)
            {
                Debug.LogError("Phone Done Prefab is null");
            }
        }

#endif
    }
}
