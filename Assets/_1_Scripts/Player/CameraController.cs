using Unity.Netcode;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace NomadsPlanet
{
    public class CameraController : NetworkBehaviour
    {
        public float sensitivity = 100.0f; // 마우스 감도
        public float clampAngleVertical = 80.0f; // 수직 마우스 움직임 제한 각도
        public float clampAngleHorizontal = 80.0f; // 수평 마우스 움직임 제한 각도

        private float _verticalRotation;
        private float _horizontalRotation;
        private float _yawRotation;

        private Transform _transform;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        private void Start()
        {
            Vector3 rotation = _transform.localRotation.eulerAngles;
            _horizontalRotation = rotation.y;
            _verticalRotation = rotation.x;
            _yawRotation = rotation.z;
        }

        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }

            float mouseX = CrossPlatformInputManager.GetAxis("Mouse X");
            float mouseY = -CrossPlatformInputManager.GetAxis("Mouse Y");

            _horizontalRotation += mouseX * sensitivity * Time.deltaTime;
            _verticalRotation += mouseY * sensitivity * Time.deltaTime;

            _verticalRotation = Mathf.Clamp(_verticalRotation, -clampAngleVertical, clampAngleVertical);
            _horizontalRotation = Mathf.Clamp(_horizontalRotation, -clampAngleHorizontal, clampAngleHorizontal);

            Quaternion targetRotation = _transform.parent.rotation *
                                        Quaternion.Euler(_verticalRotation, _horizontalRotation, _yawRotation);
            _transform.rotation = targetRotation;
        }
    }
}