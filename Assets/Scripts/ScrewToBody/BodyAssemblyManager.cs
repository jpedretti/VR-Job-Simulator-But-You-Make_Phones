using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

#if UNITY_EDITOR

using Unity.VisualScripting;

#endif

namespace com.NW84P
{
    public class BodyAssemblyManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _screwPrefab;

        [SerializeField]
        private GameObject _assembledPhonePrefab;

        [SerializeField]
        private Transform _bodyBaseBack;

        private GameObject[] _screwSpawnPoints;
        private List<Screw> _screws;
        private byte _screwedCount;
        private List<IXRSelectInteractable> _interactables;
        private CustomSocket _customSocket;

        public void Start()
        {
            InitScrewSpawnPoints();
            if (TryGetComponent(out _customSocket))
            {
                _interactables = _customSocket.interactablesSelected;
            }

            _screws = new List<Screw>(_screwSpawnPoints.Length);
        }

        public void OnDestroy() => DestroyAll();

        public void Spawn()
        {
            Screw screw;
            for (var i = 0; i < _screwSpawnPoints.Length; i++)
            {
                screw = Instantiate(_screwPrefab, _screwSpawnPoints[i].transform).GetComponent<Screw>();
                screw.OnScrewed.AddListener(OnScrewScrewed);
                _screws.Add(screw);
            }
        }

        public void DestroyAll()
        {
            foreach (var screw in _screws)
            {
                screw.OnScrewed.RemoveListener(OnScrewScrewed);
                Destroy(screw.gameObject);
            }

            _screws.Clear();
        }

        private void InitScrewSpawnPoints()
        {
            var spawnPoints = new List<GameObject>();
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);

                if (child.gameObject.CompareTag(Tags.ScrewSpawnPoint) && child.gameObject.activeSelf)
                {
                    spawnPoints.Add(child.gameObject);
                }
            }

            _screwSpawnPoints = spawnPoints.ToArray();
        }

        private void OnScrewScrewed()
        {
            _screwedCount++;
            if (_screwedCount == _screws.Count)
            {
                StartCoroutine(OnScrewedCoroutine());
            }

            if (_screwedCount == 1)
            {
                for (var i = 0; i < _interactables.Count; i++)
                {
                    var interactor = _interactables[i] as XRGrabInteractable;
                    if (interactor != null)
                    {
                        interactor.interactionLayers = LayerMasks.WELDED_MAINBOARD;
                    }
                }
            }
        }

        private IEnumerator OnScrewedCoroutine()
        {
            yield return null;
            DestroyAll();
            SwapPhoneBodies();
        }

        private void SwapPhoneBodies()
        {
            Instantiate(_assembledPhonePrefab, transform.position, _bodyBaseBack.rotation);
            DestroyRotateInteractableScript();
            DestroySelectedInteractables();
            Destroy(gameObject);
        }

        // Destroy all the selected interactables of the CustomSocket
        private void DestroySelectedInteractables()
        {
            for (var i = 0; i < _interactables.Count; i++)
            {
                _customSocket.CanSelect(_interactables[i]);
                Destroy(_interactables[i].transform.gameObject);
            }
        }

        private void DestroyRotateInteractableScript()
        {
            // Destroy the RotateInteractable of the BodySupport Object (parent of this gameObject)
            if (transform.parent.gameObject.TryGetComponent<RotateInteractable>(out var interactable))
            {
                Destroy(interactable);
            }
        }

#if UNITY_EDITOR

        public void OnValidate()
        {
            Debug.Assert(_screwPrefab != null, "Screw Prefab is not set");
            Debug.Assert(_assembledPhonePrefab != null, "Assembled Phone Prefab is not set");
            Debug.Assert(_bodyBaseBack != null || this.IsPrefabDefinition(), "Body Base Back is not set");
            InitScrewSpawnPoints();
            Debug.Assert(_screwSpawnPoints.Length > 0, "Screw Spawn Points are not set");
        }

#endif
    }
}
