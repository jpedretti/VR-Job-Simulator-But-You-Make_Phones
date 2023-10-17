using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.NW84P
{
    public class FrontGlassController : MonoBehaviour
    {
        private readonly Dictionary<string, Target> _targets = new();

        private int _targetsHitToMakeGlassUngrabbable;

        public Action OnGlassFixated { get; set; }

        public bool IsGlassFixed { get; private set; } = false;

        public void OnEnable()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.gameObject.CompareTag(Tags.Target) && child.gameObject.TryGetComponent<Target>(out var target))
                {
                    target.OnSuccessfulHit += TargetHit;
                    _targets.Add(target.gameObject.name, target);
                }
            }

            _targetsHitToMakeGlassUngrabbable = _targets.Count - 1;
        }

        public void OnDisable()
        {
            foreach (var target in _targets.Values)
            {
                target.OnSuccessfulHit -= TargetHit;
            }

            _targets.Clear();
        }

        public void EnableTargets(bool enable)
        {
            foreach (var target in _targets.Values)
            {
                target.gameObject.SetActive(enable);
            }
        }

        private void TargetHit(string targetName)
        {
            if (_targets.Remove(targetName, out var target))
            {
                target.OnSuccessfulHit -= TargetHit;
                Destroy(target.gameObject);
            }

            if (_targets.Count == _targetsHitToMakeGlassUngrabbable)
            {
                GetComponent<XRGrabInteractable>().interactionLayers = LayerMasks.FRONT_GLASS;
            }

            if (_targets.Count == 0)
            {
                IsGlassFixed = true;
                OnGlassFixated?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}
