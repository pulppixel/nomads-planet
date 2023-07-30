using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NomadsPlanet
{
    [CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/Input Reader", order = 0)]
    public class InputReader : ScriptableObject, Controls.IPlayerActions
    {
        public event Action<bool> OnPrimaryEvent;

        private Controls controls;

        private void OnEnable()
        {
            if (controls == null)
            {
                controls = new Controls();
                controls.Player.SetCallbacks(this);
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            
        }
    }
}