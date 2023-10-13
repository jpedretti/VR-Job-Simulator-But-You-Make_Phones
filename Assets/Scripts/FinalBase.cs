using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

namespace com.NW84P
{
    [RequireComponent(typeof(CustomSocket))]
    public class FinalBase : MonoBehaviour
    {
        private CustomSocket _socket;

        private void Awake() => _socket = GetComponent<CustomSocket>();

        public void OnEnable() => _socket.selectEntered.AddListener(InteractableSelected);

        public void OnDisable() => _socket.selectEntered.RemoveListener(InteractableSelected);

        private void InteractableSelected(SelectEnterEventArgs args)
        {
            var phone = args.interactableObject.transform.gameObject;
            RemoveInteractableScripts(phone);
            AddCustomSocketScript(phone);
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

        private static void AddCustomSocketScript(GameObject phone)
        {
            var customSocket = phone.AddComponent<CustomSocket>();
            customSocket.hoverSocketSnapping = false;
            customSocket.OnlySelectIfSnapped = true;
            customSocket.showInteractableHoverMeshes = false;
            customSocket.interactionLayers = LayerMasks.FRONT_GLASS;
            for (int i = 0; i < phone.transform.childCount; i++)
            {
                var child = phone.transform.GetChild(i);
                if (child.gameObject.CompareTag(Tags.Socket))
                {
                    customSocket.attachTransform = child;
                }
            }
        }

        private static void RemoveInteractableScripts(GameObject phone)
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
    }
}
