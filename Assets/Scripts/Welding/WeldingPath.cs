using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.NW84P
{
    [RequireComponent(typeof(CustomSocket))]
    [RequireComponent(typeof(AudioSource))]
    public class WeldingPath : MonoBehaviour
    {
        [SerializeField]
        private GameObject _pathPointsParent;

        [SerializeField]
        private UnityEvent _onWelded = new();

        private AudioSource _weldingAudioSource;
        private readonly Dictionary<string, GameObject> _pathPoints = new();
        private byte _weldedPoints;
        private byte _pointsBeingWelded;
        private CustomSocket _mainboard;
        private XRGrabInteractable _battery;

        private void Start()
        {
            _weldingAudioSource = GetComponent<AudioSource>();
            for (int i = 0; i < _pathPointsParent.transform.childCount; i++)
            {
                var child = _pathPointsParent.transform.GetChild(i).gameObject;
                _pathPoints.Add(child.name, child);
            }
        }

        public void OnEnable()
        {
            _mainboard = GetComponent<CustomSocket>();
            if (_mainboard.hasSelection)
            {
                _battery = _mainboard.interactablesSelected[0] as XRGrabInteractable;
            }
            _pointsBeingWelded = 0;
        }

        public void OnDisable()
        {
            _weldingAudioSource.Stop();
            _mainboard = null;
            _battery = null;
        }

        public void Weld(string gameObjectName)
        {
            if (_pathPoints.Remove(gameObjectName))
            {
                _weldedPoints++;
                if (_pathPoints.Count == 0)
                {
                    _onWelded.Invoke();
                }
            }

            if (_weldedPoints == 1 && _battery != null)
            {
                _battery.interactionLayers = LayerMasks.BATTERY;
            }
        }

        public void StartedWelding()
        {
            _pointsBeingWelded++;
            if (_pointsBeingWelded == 1)
            {
                _weldingAudioSource.Play();
            }
        }

        public void StoppedWelding()
        {
            _pointsBeingWelded--;
            if (_pointsBeingWelded == 0)
            {
                _weldingAudioSource.Stop();
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void OnValidate()
        {
            if (_pathPointsParent == null)
            {
                Debug.LogError($"PathPointsParent is null on {gameObject.name}");
            }
        }
    }
}
