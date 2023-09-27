using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.NW84P
{
    public class MainboardSocket : XRSocketInteractor
    {
        private bool _isSnapped;
        private IXRSelectInteractor _hand;
        private XRGrabInteractable _battery;

        private const float _SNAPPING_ALIGNMENT_THRESHOLD = 0.994f;
        private const float _UNSNAPPING_DISTANCE_THRESHOLD = 1f;

        protected override void OnEnable()
        {
            base.OnEnable();
            hoverSocketSnapping = false;
            hoverEntered.AddListener(HoverStarted);
            hoverExited.AddListener(HoverEnded);
            selectExited.AddListener(BatteryDeselected);
        }

        protected override void OnDisable()
        {
            hoverEntered.RemoveListener(HoverStarted);
            hoverExited.RemoveListener(HoverEnded);
            selectExited.RemoveListener(BatteryDeselected);
            base.OnDisable();
        }

        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractor(updatePhase);

            if (_battery != null && _hand != null && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                var batteryTransform = _battery.transform;
                StartSnap(batteryTransform);
                EndSnap(batteryTransform);
            }
        }

        private void EndSnap(Transform batteryTransform)
        {
            if (_isSnapped && Vector3.Distance(batteryTransform.position, _hand.transform.position) > _UNSNAPPING_DISTANCE_THRESHOLD)
            {
                EndSocketSnapping(_battery);
                _isSnapped = false;
            }
        }

        private void StartSnap(Transform batteryTransform)
        {
            var batteryLocalUp = batteryTransform.InverseTransformDirection(Vector3.up);
            var attachLocalUp = attachTransform.InverseTransformDirection(Vector3.up);

            if (!_isSnapped && Vector3.Dot(batteryLocalUp, attachLocalUp) >= _SNAPPING_ALIGNMENT_THRESHOLD)
            {
                StartSocketSnapping(_battery);
                _isSnapped = true;
            }
        }

        private void BatteryDeselected(SelectExitEventArgs args) => _isSnapped = false;

        private void HoverStarted(HoverEnterEventArgs args)
        {
            _battery = args.interactableObject as XRGrabInteractable;

            if(_battery == null) return;
            if(_battery.interactorsSelecting.Count == 0) return;

            _battery.selectExited.AddListener(BatterySelectExited);
            _hand = _battery.interactorsSelecting[0];
        }

        private void HoverEnded(HoverExitEventArgs args)
        {
            _isSnapped = false;
            Cleanup();
        }

        private void BatterySelectExited(SelectExitEventArgs args)
        {
            if (args.interactableObject == _hand)
            {
                Cleanup();
            }
        }

        private void Cleanup()
        {
            _hand = null;

            if (_battery != null)
            {
                _battery.selectExited.RemoveListener(BatterySelectExited);
                _battery = null;
            }
        }
    }
}
