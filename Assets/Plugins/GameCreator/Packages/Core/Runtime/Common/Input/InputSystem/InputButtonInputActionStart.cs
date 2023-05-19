using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Input Action Start (Button)")]
    [Category("Input System/Input Action Start (Button)")]
    
    [Description("When an Input Action asset of Button type runs the Started phase")]
    [Image(typeof(IconBoltOutline), ColorTheme.Type.Blue, typeof(OverlayArrowLeft))]
    
    [Keywords("Unity", "Asset", "Map", "Press")]
    
    [Serializable]
    public class InputButtonInputActionStart : TInputButton
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private InputActionFromAsset m_Input = new InputActionFromAsset();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override bool Active
        {
            get => this.m_Input.InputAction?.enabled ?? false;
            set
            {
                switch (value)
                {
                    case true: this.Enable(); break;
                    case false: this.Disable(); break;
                }
            }
        }

        // INITIALIZERS: --------------------------------------------------------------------------
        
        public override void OnStartup()
        {
            this.Enable();
        }

        public override void OnDispose()
        {
            this.Disable();
            this.m_Input.InputAction?.Dispose();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void Enable()
        {
            if (this.m_Input.InputAction == null) return;
            if (this.m_Input.InputAction.enabled) return;
            
            this.m_Input.InputAction.Enable();
            this.m_Input.InputAction.started += this.ExecuteEventStart;
            this.m_Input.InputAction.canceled += this.ExecuteEventCancel;
        }

        private void Disable()
        {
            if (this.m_Input.InputAction is not { enabled: true }) return;

            this.m_Input.InputAction.Disable();
            this.m_Input.InputAction.started -= this.ExecuteEventStart;
            this.m_Input.InputAction.canceled -= this.ExecuteEventCancel;
        }
        
        private void ExecuteEventStart(InputAction.CallbackContext context)
        {
            this.ExecuteEventStart();
            this.ExecuteEventPerform();
        }
        
        private void ExecuteEventCancel(InputAction.CallbackContext context)
        {
            this.ExecuteEventCancel();
        }
    }
}