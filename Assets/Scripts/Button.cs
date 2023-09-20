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
        private Vector3 m_ButtonOriginalPosition = Vector3.negativeInfinity;
        private float m_EntryPointY = float.PositiveInfinity;
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
                m_ButtonOriginalPosition = m_ButtonTransform.position;
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
            m_InteractorTransform = null;
            IsPressed = false;
            m_EntryPointY = float.PositiveInfinity;
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
            var diff = Mathf.Max(0, m_EntryPointY - m_InteractorTransform.position.y - m_ButtonOffset);
            var newHeight = Mathf.Clamp(m_ButtonOriginalPosition.y - diff, m_ButtonPressLimit, m_ButtonOriginalPosition.y);
            m_ButtonTransform.position = new(m_ButtonOriginalPosition.x, newHeight, m_ButtonOriginalPosition.z);
            return newHeight;
        }

        private void SetEntryPoint()
        {
            if (float.IsPositiveInfinity(m_EntryPointY))
            {
                m_EntryPointY = m_InteractorTransform.position.y;
            }
        }

        private float GetPressLimit() => m_ButtonOriginalPosition.y - m_DistanceToBePressed;

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
                m_IsCorrectSide = m_InteractorTransform.position.y > m_ButtonOriginalPosition.y;
            }
        }

        private void HoverEnded(HoverExitEventArgs args)
        {
            // do this on disable?
            if (m_InteractorTransform != null)
            {
                m_InteractorTransform = null;
                m_EntryPointY = float.PositiveInfinity;
                m_ButtonTransform.position = m_ButtonOriginalPosition;
                InvokeAction(pressed: false, () => OnButtonReleased.Invoke());
            }
        }

#if UNITY_EDITOR

        public void OnValidate()
        {
            if (m_ButtonOriginalPosition != Vector3.negativeInfinity)
            {
                m_ButtonPressLimit = GetPressLimit();
            }
        }
    }

#endif
}
