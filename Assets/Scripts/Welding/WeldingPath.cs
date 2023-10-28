using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace com.NW84P
{
    public class WeldingPath : MonoBehaviour
    {
        [SerializeField]
        private GameObject _pathPointsParent;

        [SerializeField]
        private UnityEvent _onWelded = new();

        private AudioSource _weldingAudioSource;
        private readonly Dictionary<string, GameObject> _pathPoints = new();
        private byte _pointsBeingWelded;

        private void Start()
        {
            _weldingAudioSource = GetComponent<AudioSource>();
            for (int i = 0; i < _pathPointsParent.transform.childCount; i++)
            {
                var child = _pathPointsParent.transform.GetChild(i).gameObject;
                _pathPoints.Add(child.name, child);
            }
        }

        public void OnEnable() => _pointsBeingWelded = 0;

        public void OnDisable() => _weldingAudioSource.Stop();

        public void Weld(string gameObjectName)
        {
            if (_pathPoints.Remove(gameObjectName) && _pathPoints.Count == 0)
            {
                _onWelded.Invoke();
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
