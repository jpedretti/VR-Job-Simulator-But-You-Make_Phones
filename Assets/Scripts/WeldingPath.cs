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

        private readonly Dictionary<string, GameObject> _pathPoints = new();

        private void Start()
        {
            for (int i = 0; i < _pathPointsParent.transform.childCount; i++)
            {
                var child = _pathPointsParent.transform.GetChild(i).gameObject;
                _pathPoints.Add(child.name, child);
            }
        }

        public void Weld(string gameObjectName)
        {
            if (_pathPoints.Remove(gameObjectName) && _pathPoints.Count == 0)
            {
                _onWelded.Invoke();
            }
        }

#if UNITY_EDITOR

        public void OnValidate()
        {
            if (_pathPointsParent == null)
            {
                Debug.LogError($"PathPointsParent is null on {gameObject.name}");
            }
        }

#endif
    }
}
