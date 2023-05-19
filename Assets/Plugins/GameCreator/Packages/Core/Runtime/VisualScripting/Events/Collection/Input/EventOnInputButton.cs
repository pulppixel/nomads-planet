using System;
using GameCreator.Runtime.Common;
using UnityEngine.Scripting.APIUpdating;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Input Button")]
    [Category("Input/On Input Button")]
    [Description("Detects when a button is interacted with")]
    
    [Image(typeof(IconButton), ColorTheme.Type.Yellow)]
    [Keywords("Down", "Up", "Press", "Release")]
    [Keywords("Keyboard", "Mouse", "Button", "Gamepad", "Controller", "Joystick")]

    // TODO: [10/3/2023] Remove in a year
    [MovedFrom(false, null, null, "EventOnInput")]
    
    [Serializable]
    public class EventOnInputButton : TEventButton
    {
        protected override void OnInput()
        {
            base.OnInput();
            this.Execute();
        }
    }
}