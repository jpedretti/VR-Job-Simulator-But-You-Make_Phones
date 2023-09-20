using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace com.NW84P
{
    public class Button : XRBaseInteractable
    {
        [Header("Button Configuration")]
        [SerializeField]
        private Transform m_ButtonTransform;

        [SerializeField]
        private float m_ButtonOffset = 0;

        [SerializeField]
        private float m_DistanceToBePressed;

        [SerializeField]
        private UnityEvent m_OnButtonPressed = new();

        [SerializeField]
        private UnityEvent m_OnButtonReleased = new();

        private Transform m_InteractorTransform = null;
        private Vector3 m_ButtonOriginalLocalPosition = Vector3.positiveInfinity;
        private Vector3 m_EntryPoint = Vector3.positiveInfinity;
        private bool m_IsCorrectSide = false;
        private float m_ButtonPressLimit = 0;

        public UnityEvent OnButtonPressed => m_OnButtonPressed;
        public UnityEvent OnButtonReleased => m_OnButtonReleased;
        public bool IsPressed { get; private set; } = false;

        protected override void OnEnable()
        {
            base.OnEnable();
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

        protected override void ProcessInteractionStrength(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractionStrength(updatePhase);
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                if (m_InteractorTransform != null && m_IsCorrectSide)
                {
                    SetEntryPoint();
                    float newHeight = CalculateButtonHeight();
                    InvokeEvents(pressed: newHeight <= m_ButtonPressLimit);
                }
            }
        }

        private void InvokeEvents(bool pressed)
        {
            if (pressed)
            {
                InvokeAction(pressed, () => OnButtonPressed.Invoke());
            }
            else
            {
                InvokeAction(pressed, () => OnButtonReleased.Invoke());
            }
        }

        private float CalculateButtonHeight()
        {
            var distance = transform.InverseTransformDirection(m_EntryPoint - m_InteractorTransform.position);
            var diff = Mathf.Max(0, distance.y - m_ButtonOffset);
            var newHeight = Mathf.Clamp(m_ButtonOriginalLocalPosition.y - diff, m_ButtonPressLimit, m_ButtonOriginalLocalPosition.y);
            m_ButtonTransform.localPosition = new(m_ButtonOriginalLocalPosition.x, newHeight, m_ButtonOriginalLocalPosition.z);
            return newHeight;
        }

        private void SetEntryPoint()
        {
            if (m_EntryPoint.y == Vector3.positiveInfinity.y)
            {
                m_EntryPoint = m_InteractorTransform.position;
            }
        }

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

                // improve is correct side check
                VerifyCorrectSize();
            }
        }

        private void VerifyCorrectSize()
        {
            if (!m_IsCorrectSide)
            {
                var interactorLocalPosition = m_ButtonTransform.InverseTransformDirection(m_InteractorTransform.position);
                m_IsCorrectSide = interactorLocalPosition.y > m_ButtonOriginalLocalPosition.y;
            }
        }

        private void HoverEnded(HoverExitEventArgs args)
        {
            // do this on disable?
            if (m_InteractorTransform != null)
            {
                m_ButtonTransform.localPosition = m_ButtonOriginalLocalPosition;
                ResetState(triggerOnButtonRelease: true);
            }
        }

        private void ResetState(bool triggerOnButtonRelease)
        {
            m_InteractorTransform = null;
            m_EntryPoint = Vector3.positiveInfinity;
            m_IsCorrectSide = false;
            if(!triggerOnButtonRelease)
            {
                IsPressed = false;
            }
            InvokeAction(pressed: false, () => OnButtonReleased.Invoke());
        }

#if UNITY_EDITOR

        public void OnValidate()
        {
            if (m_ButtonOriginalLocalPosition != Vector3.positiveInfinity)
            {
                m_ButtonPressLimit = GetPressLimit();
            }
        }
    }

#endif
}
