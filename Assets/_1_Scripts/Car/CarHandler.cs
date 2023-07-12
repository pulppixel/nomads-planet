using System;
using UnityEngine;
using NomadsPlanet.Utils;
using UnityStandardAssets.Vehicles.Car;

namespace NomadsPlanet
{
    public class CarHandler : MonoBehaviour
    {
        private CarController _carController;
        public LayerMask layerMask;

        private const float MaxDistance = 20f;
        private const float SlowerDistance = 10f;

        private LaneType currentLane;
        private LaneType targetLane;
        private TrafficType targetTraffic;

        internal enum CarState
        {
            Idle,       // 정지 상태
            ChangeLane, // 차선 변경 상태
            Drive,      // 드라이브 상태
            Stopping,   // 브레이크 밟은 상태
        }
        
        private void Awake()
        {
            _carController = GetComponent<CarController>();
        }

        private void Update()
        {
            CarMovement();
            StoppingControl(DetectOtherCar());
        }

        private void OnTriggerEnter(Collider other)
        {
            // 현재 달리고 있는 차선을 받아온다.
            // if (other.TryGetComponent<LaneController>(out var roadController))
            // {
            //     // Debug.Log(roadController.LaneType.ToString());
            // }

            // 목표하고 있는 방향을 정한다.
            if (other.TryGetComponent<TrafficFlow>(out var trafficFlow))
            {
                targetTraffic = trafficFlow.TrafficType;
                // Debug.Log(trafficFlow.curLightType.ToString());
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent<TrafficFlow>(out var trafficFlow))
            {
                // Debug.Log(trafficFlow.currentLightType.ToString());
            }
        }

        private float DetectOtherCar()
        {
            var tr = GetComponent<Transform>();
            if (Physics.Raycast(tr.position, tr.forward, out var hit, MaxDistance, layerMask))
            {
                // Debug.Log("Player Detected! Distance: " + hit.distance);

                return hit.distance;
            }

            return 0f;

        }

        private static void StoppingControl(float distance)
        {
            if (!(Math.Abs(distance) > 0.001f)) return;

            // 만약 거리가 너무 가까우면 속도를 줄이거나 정지하게 합니다.
            if (distance < SlowerDistance)
            {
                // Debug.Log("Too Close! Slowing down or stopping...");
                // 여기에 차량의 속도를 조절하는 코드를 작성하세요.
            }
        }

        private void CarMovement()
        {
            // todo: 점진적으로 0~1로 갈 수 있도록 하기 DOTWeen 사용하면 돼
            // _carController.Move(-.05f, .1f, .1f, 0);
        }
    }
}