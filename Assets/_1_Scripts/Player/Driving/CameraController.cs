using Cinemachine;
using UnityEngine;
using Unity.Netcode;

#if UNITY_ANDROID || UNITY_IOS
using DigitalRubyShared;
#endif

namespace NomadsPlanet
{
    public class CameraController : NetworkBehaviour
    {
        private const float Sensitivity = 200.0f;
        private const float ClampAngleVertical = 45.0f;
        private const float ClampAngleHorizontal = 60.0f;

        private float _verticalRotation;
        private float _horizontalRotation;
        private float _yawRotation;

        private CinemachineVirtualCamera _virtualCamera;
        private Transform _transform;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }

        private void Start()
        {
            Vector3 rotation = _transform.localRotation.eulerAngles;
            _horizontalRotation = rotation.y;
            _verticalRotation = rotation.x;
            _yawRotation = rotation.z;

            if (!IsOwner)
            {
                _virtualCamera.Priority = -5;
            }
        }

        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }

#if UNITY_ANDROID || UNITY_IOS
            float mouseX = FingersJoystickScript.Instance.CurrentAmount.x;
            float mouseY = -FingersJoystickScript.Instance.CurrentAmount.y;
#else
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = -Input.GetAxis("Mouse Y");
#endif

            _horizontalRotation += AdjustSensitivity(
                mouseX * Sensitivity * Time.deltaTime,
                _horizontalRotation,
                -ClampAngleHorizontal,
                ClampAngleHorizontal
            );

            _verticalRotation += AdjustSensitivity(
                mouseY * Sensitivity * Time.deltaTime,
                _verticalRotation,
                -ClampAngleVertical,
                ClampAngleVertical
            );

            _verticalRotation = Mathf.Clamp(_verticalRotation, -ClampAngleVertical, ClampAngleVertical);
            _horizontalRotation = Mathf.Clamp(_horizontalRotation, -ClampAngleHorizontal, ClampAngleHorizontal);

            Quaternion targetRotation = _transform.parent.rotation *
                                        Quaternion.Euler(_verticalRotation, _horizontalRotation, _yawRotation);
            _transform.rotation = targetRotation;
        }

        private static float AdjustSensitivity(float input, float currentRotation, float minAngle, float maxAngle)
        {
            float distanceToLowerBound = Mathf.Abs(currentRotation - minAngle);
            float distanceToUpperBound = Mathf.Abs(currentRotation - maxAngle);

            float factor = Mathf.Min(distanceToLowerBound, distanceToUpperBound) / (maxAngle - minAngle);
            return input * Mathf.SmoothStep(0.5f, 1f, factor);
        }
    }
}