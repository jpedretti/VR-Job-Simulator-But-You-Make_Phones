using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.NW84P
{
    public class WeldedMainBoardSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject _weldedMainBoardPrefab;

        [SerializeField]
        private Transform _playingGameObjectsParent;

        public void Spawn()
        {
            Instantiate(_weldedMainBoardPrefab, transform.position, transform.rotation, _playingGameObjectsParent);
            DestroyBattery();
            Destroy(gameObject);
        }

        private void DestroyBattery()
        {
            if (TryGetComponent<XRSocketInteractor>(out var interactor))
            {
                var interactors = interactor.interactablesSelected;
                for (int i = 0; i < interactors.Count; i++)
                {
                    if (interactors[i].transform.gameObject.CompareTag(Tags.Battery))
                    {
                        Destroy(interactors[i].transform.gameObject);
                        break;
                    }
                }
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void OnValidate()
        {
            if (_weldedMainBoardPrefab == null)
            {
                Debug.LogError($"WeldedMainBoardSpawner is null on {gameObject.name}");
            }
        }
    }
}
