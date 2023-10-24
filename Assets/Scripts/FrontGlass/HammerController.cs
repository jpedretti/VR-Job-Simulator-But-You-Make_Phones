using UnityEngine;

namespace com.NW84P
{
    public class HammerController : MonoBehaviour
    {
        private const float _MAX_VOLUME = 0.9f;
        private const float _MIN_VOLUME = 0.1f;
        private AudioSource _audioSource;
        private Rigidbody _hammerRigidbody;

        private void Start()
        {
            _audioSource = GetComponentInChildren<AudioSource>();
            _hammerRigidbody = GetComponent<Rigidbody>();
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(Tags.FrontGlass) || collision.gameObject.CompareTag(Tags.PhoneDone))
            {
                var projectedVelocity = Vector3.Project(_hammerRigidbody.velocity, collision.transform.forward);
                _audioSource.volume = Mathf.Lerp(_MIN_VOLUME, _MAX_VOLUME, projectedVelocity.magnitude);
                if (!_audioSource.isPlaying)
                {
                    _audioSource.Play();
                }
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]

        public void OnValidate()
        {
            if (GetComponent<Rigidbody>() == null)
            {
                Debug.LogError("HammerController needs a Rigidbody component");
            }

            if (GetComponentInChildren<AudioSource>() == null)
            {
                Debug.LogError("HammerController needs an AudioSource component");
            }
        }
    }
}
