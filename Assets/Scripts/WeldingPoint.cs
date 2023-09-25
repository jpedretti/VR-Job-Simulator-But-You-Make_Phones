using System.Collections;
using UnityEngine;

namespace com.NW84P
{
    public class WeldingPoint : MonoBehaviour
    {
        private WeldingPath _weldingPath;

        private void Start() => _weldingPath = GetComponentInParent<WeldingPath>();

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tags.TorchFire))
            {
                Debug.Log($"Welding point entered! {gameObject.name}");
                StartCoroutine(Weld());
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Tags.TorchFire))
            {
                Debug.Log($"Welding point exited! {gameObject.name}");
                StopAllCoroutines();
            }
        }

        private IEnumerator Weld()
        {
            yield return new WaitForSeconds(2);
            _weldingPath.Weld(name);
        }
    }
}
