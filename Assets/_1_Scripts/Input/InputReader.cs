using UnityEngine;
using UnityEngine.InputSystem;

namespace NomadsPlanet
{
    [CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/Input Reader", order = 0)]
    public class InputReader : ScriptableObject, Controls.IPlayerActions
    {
        // public event Action<bool> OnPrimaryEvent;

        private Controls _controls;

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.Player.SetCallbacks(this);
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
        }
    }
}