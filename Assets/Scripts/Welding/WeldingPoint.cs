using System.Collections;
using UnityEngine;

namespace com.NW84P
{
    public class WeldingPoint : MonoBehaviour
    {
        private const float _TIME_TO_WELD = 0.6f;

        private WeldingPath _weldingPath;
        private ParticleSystem _fireParticle;
        private LineRenderer _lineRenderer;
        private bool _isWelded;

        private void Start()
        {
            _weldingPath = GetComponentInParent<WeldingPath>();
            _fireParticle = GetComponent<ParticleSystem>();
            _lineRenderer = GetComponent<LineRenderer>();
        }

        public void OnDisable()
        {
            StopWelding();
            DestroyWeldingPoint();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!_isWelded && other.CompareTag(Tags.TorchFire))
            {
                _fireParticle.Play();
                StartCoroutine(Weld());
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Tags.TorchFire))
            {
                StopWelding();
            }
        }

        private void StopWelding()
        {
            if (!_isWelded)
            {
                _weldingPath.StoppedWelding();
                _fireParticle.Stop();
                StopAllCoroutines();
            }
        }

        public void OnParticleSystemStopped() => DestroyWeldingPoint();

        private void DestroyWeldingPoint()
        {
            if (_isWelded)
            {
                Destroy(gameObject);
            }
        }

        private IEnumerator Weld()
        {
            _weldingPath.StartedWelding();
            yield return WaitForSecondsCache.Get(_TIME_TO_WELD);
            _isWelded = true;
            _fireParticle.Stop();
            _lineRenderer.enabled = false;
            _weldingPath.Weld(name);
            _weldingPath.StoppedWelding();
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void OnValidate()
        {
            if (GetComponent<ParticleSystem>() == null)
            {
                Debug.LogError($"FireParticle is null on {gameObject.name}");
            }
            if (GetComponent<LineRenderer>() == null)
            {
                Debug.LogError($"LineRenderer is null on {gameObject.name}");
            }
        }
    }
}
