using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.NW84P
{
    [RequireComponent(typeof(CustomSocket))]
    public class SimSlotController : MonoBehaviour
    {
        private const float _SIM_FINAL_X_POSITION = -0.01029f;
        private const float _SIM_INSERTION_DURATION = 0.5f;

        [SerializeField]
        private GameObject _insertedSimCardPrefab;

        private CustomSocket _simSlot;

        public void OnEnable()
        {
            _simSlot = GetComponent<CustomSocket>();
            _simSlot.selectEntered.AddListener(SimAttached);
        }

        public void OnDisable() => _simSlot.selectEntered.RemoveListener(SimAttached);

        private void SimAttached(SelectEnterEventArgs args) => SetUpInsertedSim(args.interactableObject);

        private void SetUpInsertedSim(IXRSelectInteractable interactable)
        {
            var simCardTransform = interactable.transform;
            var insertedSim = Instantiate(_insertedSimCardPrefab, simCardTransform.position, simCardTransform.rotation, transform);
            Destroy(interactable.transform.gameObject);
            StartCoroutine(CardInsetionsAnimation(insertedSim.transform));
        }

        private IEnumerator CardInsetionsAnimation(Transform insertedSimTransform)
        {
            var elapsedTime = 0f;
            var initialPosition = insertedSimTransform.localPosition;

            while (elapsedTime < _SIM_INSERTION_DURATION)
            {
                var newXPosition = Mathf.Lerp(initialPosition.x, _SIM_FINAL_X_POSITION, elapsedTime / _SIM_INSERTION_DURATION);
                insertedSimTransform.localPosition = new Vector3(newXPosition, initialPosition.y, initialPosition.z);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Destroy(this);
        }

#if UNITY_EDITOR

        public void OnValidate()
        {
            if (_insertedSimCardPrefab == null)
            {
                Debug.LogError("SimSlotController: Inserted Sim Card Prefab is null");
            }
        }

#endif
    }
}
