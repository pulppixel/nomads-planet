using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public class MobileInputController : MonoBehaviour
    {
        public static MobileInputController Instance { get; private set; }
        [SerializeField] private GameObject inputController;

        [SerializeField] private ButtonState leftButton;
        [SerializeField] private ButtonState rightButton;
        [SerializeField] private ButtonState upButton;
        [SerializeField] private ButtonState downButton;

        public Vector2 InputValue { get; private set; }
        private const float Speed = 1.5f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
#if UNITY_ANDROID || UNITY_IOS
#else
            gameObject.SetActive(false);
            inputController.SetActive(false);
#endif
        }

        private void Update()
        {
            float targetHorizontal = 0f;
            float targetVertical = 0f;

            if (IsButtonPressed(upButton))
            {
                targetVertical = 1f;
            }
            else if (IsButtonPressed(downButton))
            {
                targetVertical = -1f;
            }

            if (IsButtonPressed(leftButton))
            {
                targetHorizontal = -1f;
            }
            else if (IsButtonPressed(rightButton))
            {
                targetHorizontal = 1f;
            }

            InputValue = new Vector2(
                Mathf.Lerp(InputValue.x, targetHorizontal, Speed * Time.deltaTime),
                Mathf.Lerp(InputValue.y, targetVertical, Speed * Time.deltaTime)
            );
        }

        private static bool IsButtonPressed(ButtonState button)
        {
            return button.isPressed;
        }
    }
}