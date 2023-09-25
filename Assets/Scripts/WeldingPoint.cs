using System.Collections;
using UnityEngine;

namespace com.NW84P
{
    public class WeldingPoint : MonoBehaviour
    {
        private WeldingPath _weldingPath;
        private ParticleSystem _fireParticle;
        private LineRenderer _lineRenderer;

        private void Start()
        {
            _weldingPath = GetComponentInParent<WeldingPath>();
            _fireParticle = GetComponent<ParticleSystem>();
            _lineRenderer = GetComponent<LineRenderer>();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tags.TorchFire))
            {
                _fireParticle.Play();
                StartCoroutine(Weld());
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Tags.TorchFire))
            {
                _fireParticle.Stop();
                StopAllCoroutines();
            }
        }

        public void OnParticleSystemStopped()
        {
            if (!_lineRenderer.enabled)
            {
                Destroy(gameObject);
            }
        }

        private IEnumerator Weld()
        {
            yield return new WaitForSeconds(2);
            _lineRenderer.enabled = false;
            _fireParticle.Stop();
            _weldingPath.Weld(name);
        }
    }
}
