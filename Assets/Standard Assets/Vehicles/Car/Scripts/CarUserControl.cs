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

            float horizontal = 0f;
            float vertical = 0f;
            float handbrake = 0f;

            // pass the input to the car!
#if UNITY_ANDROID || UNITY_IOS
            if (MobileInputController.Instance != null)
            {
                horizontal = MobileInputController.Instance.InputValue.x;
                vertical = MobileInputController.Instance.InputValue.y;
                handbrake = 0f;
            }
#else
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            handbrake = CrossPlatformInputManager.GetAxis("Jump");
#endif

            m_Animatior.SetBool("TurnLeft", horizontal < -.1f);
            m_Animatior.SetBool("TurnRight", horizontal > .1f);

            m_Car.Move(horizontal, vertical, vertical, handbrake);
        }
    }
}