using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.NW84P
{
    /// <summary>
    /// Button is a custom XR interactable class that represents a pressable button. It inherits from XRBaseInteractable
    /// and provides functionality for detecting button press and release events. The class exposes UnityEvents for
    /// button press and release actions, allowing users to easily configure custom behaviors.
    /// </summary>
    public class Button : XRBaseInteractable
    {
        [Header("Button Configuration")]
        [Tooltip("The transform of the button object. Needs to be a child of this object")]
        [SerializeField]
        private Transform m_ButtonTransform;

        [Tooltip("The offset of the button. Adjust this value if the button starts moving sooner or later than expected")]
        [SerializeField]
        private float m_ButtonOffset;

        [Tooltip("The distance the button needs to be pushed to be considered as pressed.")]
        [SerializeField]
        private float m_DistanceToBePressed;

        [Tooltip("The event triggered when the button is pressed.")]
        [SerializeField]
        private UnityEvent m_OnButtonPressed = new();

        [Tooltip("The event triggered when the button is released.")]
        [SerializeField]
        private UnityEvent m_OnButtonReleased = new();

        private Transform m_InteractorTransform = null;
        private Transform m_ThisTransform;
        private Vector3 m_ButtonOriginalLocalPosition = Vector3.positiveInfinity;
        private bool m_IsCorrectSide = false;
        private float m_ButtonPressLimit = 0;
        private XRBaseController _controller;

        public UnityEvent OnButtonPressed => m_OnButtonPressed;
        public UnityEvent OnButtonReleased => m_OnButtonReleased;
        public bool IsPressed { get; private set; } = false;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_ThisTransform = transform;
            if (m_ButtonTransform != null)
            {
                m_ButtonOriginalLocalPosition = m_ButtonTransform.localPosition;
                m_ButtonPressLimit = GetPressLimit();
                hoverEntered.AddListener(HoverStarted);
                hoverExited.AddListener(HoverEnded);
            }
            else
            {
                Debug.LogError($"Button Transform was not set in {gameObject}");
            }
        }

        protected override void OnDisable()
        {
            hoverEntered.RemoveListener(HoverStarted);
            hoverExited.RemoveListener(HoverEnded);
            ResetState(triggerOnButtonRelease: false);
            base.OnDisable();
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);
            if (m_InteractorTransform != null && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                var interactorLocalPosition = GetIteractorLocalPosition();
                VerifyCorrectSide(interactorLocalPosition);

                if (m_IsCorrectSide)
                {
                    float newHeight = CalculateButtonHeight(interactorLocalPosition);
                    InvokeEvents(pressed: newHeight <= m_ButtonPressLimit);
                }
            }
        }

        private void InvokeEvents(bool pressed)
        {
            if (pressed)
            {
                InvokeAction(pressed, () =>
                {
                    OnButtonPressed.Invoke();
                    _controller.SendHapticImpulse(0.5f, 0.1f);
                });
            }
            else
            {
                InvokeAction(pressed, () => OnButtonReleased.Invoke());
            }
        }

        private float CalculateButtonHeight(Vector3 interactorLocalPosition)
        {
            var distance = interactorLocalPosition - m_ButtonOriginalLocalPosition;
            var diff = Mathf.Max(0, m_ButtonOffset - distance.y);
            var newHeight = Mathf.Clamp(m_ButtonOriginalLocalPosition.y - diff, m_ButtonPressLimit, m_ButtonOriginalLocalPosition.y);
            m_ButtonTransform.localPosition = new(m_ButtonOriginalLocalPosition.x, newHeight, m_ButtonOriginalLocalPosition.z);
            return newHeight;
        }

        private Vector3 GetIteractorLocalPosition()
            => m_ThisTransform.InverseTransformPoint(m_InteractorTransform.position);

        private float GetPressLimit() => m_ButtonOriginalLocalPosition.y - m_DistanceToBePressed;

        private void InvokeAction(bool pressed, Action action)
        {
            if (IsPressed != pressed)
            {
                IsPressed = pressed;
                action?.Invoke();
            }
        }

        private void HoverStarted(HoverEnterEventArgs args)
        {
            if (m_InteractorTransform == null)
            {
                m_InteractorTransform = args.interactorObject.GetAttachTransform(this);
                _controller = args.interactorObject.GetController();
                VerifyCorrectSide(GetIteractorLocalPosition());
            }
        }

        private void VerifyCorrectSide(Vector3 interactorLocalPosition)
        {
            if (!m_IsCorrectSide)
            {
                m_IsCorrectSide = interactorLocalPosition.y > m_ButtonOriginalLocalPosition.y + m_ButtonOffset;
            }
        }

        private void HoverEnded(HoverExitEventArgs args)
        {
            if (m_InteractorTransform != null)
            {
                m_ButtonTransform.localPosition = m_ButtonOriginalLocalPosition;
                ResetState(triggerOnButtonRelease: true);
            }
        }

        private void ResetState(bool triggerOnButtonRelease)
        {
            m_InteractorTransform = null;
            m_IsCorrectSide = false;
            _controller = null;
            if (!triggerOnButtonRelease)
            {
                IsPressed = false;
            }
            InvokeAction(pressed: false, () => OnButtonReleased.Invoke());
        }

#if UNITY_EDITOR

        public void OnValidate()
        {
            if (m_ButtonOriginalLocalPosition.y != Vector3.positiveInfinity.y)
            {
                m_ButtonPressLimit = GetPressLimit();
            }

            if (m_ButtonTransform == null)
            {
                Debug.LogError($"The button transform is not set in {gameObject}");
            }

            if (!m_ButtonTransform.IsChildOf(transform))
            {
                Debug.LogError($"The button transform is not a child of {gameObject}");
            }
        }

#endif
    }
}
