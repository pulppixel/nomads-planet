﻿using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public class MobileInputController : MonoBehaviour
    {
        private static MobileInputController _instance;
        [SerializeField] private GameObject inputController;

        [SerializeField] private ButtonState leftButton;
        [SerializeField] private ButtonState rightButton;
        [SerializeField] private ButtonState upButton;
        [SerializeField] private ButtonState downButton;

        public Vector2 InputValue { get; private set; }
        private const float Speed = 1.5f;

        public static MobileInputController Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = FindObjectOfType<MobileInputController>();

                return _instance == null ? null : _instance;
            }
        }

        private void Start()
        {
#if UNITY_ANDROID || UNITY_IOS
            gameObject.SetActive(true);
            inputController.SetActive(true);
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