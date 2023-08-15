using DigitalRubyShared;
using Unity.Netcode;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarUserControl : NetworkBehaviour
    {
        [SerializeField] private Animator m_Animatior;
        private CarController m_Car; // the car controller we want to use

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }

        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }

            // pass the input to the car!
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            float horizontal = FingersJoystickScript.Instance.CurrentAmount.x;
            float vertical = FingersJoystickScript.Instance.CurrentAmount.y;
#else
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
#endif

            m_Animatior.SetBool("TurnLeft", vertical < -.1f);
            m_Animatior.SetBool("TurnRight", vertical > .1f);
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(vertical, horizontal, horizontal, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}