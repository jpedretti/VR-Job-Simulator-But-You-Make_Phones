using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.NW84P
{
    public class Handwheel : XRBaseInteractable
    {
        [Header("Handwheel Settings")]
        [SerializeField]
        private GameObject _pressCylinder;

        private List<InteractorHandlePair> _interactorHandlePairs = new(2);

        protected override void OnEnable()
        {
            base.OnEnable();

            selectMode = InteractableSelectMode.Multiple;
            selectEntered.AddListener(SelectStarted);
            selectExited.AddListener(SelectExited);
        }

        protected override void OnDisable()
        {
            selectEntered.RemoveListener(SelectStarted);
            selectExited.RemoveListener(SelectExited);
            _interactorHandlePairs.Clear();
            base.OnDisable();
        }

        private void SelectStarted(SelectEnterEventArgs args)
        {
            if (args.interactorObject.transform.gameObject.CompareTag(Tags.Player))
            {
                var direction = (colliders[0].transform.position - args.interactorObject.transform.position).normalized;
                var distance = Vector3.Distance(colliders[0].transform.position, args.interactorObject.transform.position);
                Debug.DrawRay(args.interactorObject.transform.position, direction * distance, Color.red, 5f);
                Debug.Log($"Interacted with player");

                if(Physics.Raycast(args.interactorObject.transform.position, direction, out RaycastHit hit))
                {
                    Debug.Log($"Hit {hit.collider.transform.gameObject.name}");
                    _interactorHandlePairs.Add(
                        new InteractorHandlePair(args.interactorObject, hit.collider.transform.gameObject)
                    );
                }

            }
        }

        private void SelectExited(SelectExitEventArgs args)
        {
            if (args.interactorObject.transform.gameObject.CompareTag(Tags.Player))
            {
                for (int i = 0; i < _interactorHandlePairs.Count; i++)
                {
                    if (_interactorHandlePairs[i].Interactor == args.interactorObject)
                    {
                        _interactorHandlePairs.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private readonly struct InteractorHandlePair
        {
            public InteractorHandlePair(IXRSelectInteractor interactor, GameObject handle)
            {
                _handle = handle;
                _interactor = interactor;
            }

            private readonly GameObject _handle;
            private readonly IXRSelectInteractor _interactor;

            public readonly GameObject Handle => _handle;
            public readonly IXRSelectInteractor Interactor => _interactor;
        }
    }
}
