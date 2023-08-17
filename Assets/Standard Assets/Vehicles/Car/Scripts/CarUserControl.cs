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
#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
            float horizontal = MobileInputController.Instance.InputValue.x;
            float vertical = MobileInputController.Instance.InputValue.y;
            float handbrake = 0f;
#else
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
#endif

            m_Animatior.SetBool("TurnLeft", horizontal < -.1f);
            m_Animatior.SetBool("TurnRight", horizontal > .1f);

            m_Car.Move(horizontal, vertical, vertical, handbrake);
        }
    }
}