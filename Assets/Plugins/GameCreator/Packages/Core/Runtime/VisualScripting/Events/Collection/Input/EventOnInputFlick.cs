using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Input Flick")]
    [Category("Input/On Input Flick")]
    [Description("Detects when a numeric Input is flicked")]

    [Image(typeof(IconJoystick), ColorTheme.Type.Yellow)]

    [Serializable]
    public class EventOnInputFlick : TEventValue
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private InputPropertyValueFloat m_Input = new InputPropertyValueFloat();
        [SerializeField] private CompareMinDistanceOrNone m_MinDistance = new CompareMinDistanceOrNone();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override float Value => this.m_Input.Read();
        protected override CompareMinDistanceOrNone MinDistance => this.m_MinDistance;

        // METHODS: -------------------------------------------------------------------------------

        protected internal override void OnAwake(Trigger trigger)
        {
            base.OnAwake(trigger);
            this.m_Input.OnStartup();
        }

        protected internal override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            this.m_Input.Enable();
        }

        protected internal override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);
            this.m_Input.Disable();
        }

        protected internal override void OnDestroy(Trigger trigger)
        {
            base.OnDestroy(trigger);
            this.m_Input.OnDispose();
        }

        protected internal override void OnUpdate(Trigger trigger)
        {
            base.OnUpdate(trigger);
            
            this.m_Input.OnUpdate();
            this.CheckExecute();
        }
    }
}