using System;
using NomadsPlanet.Utils;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

namespace NomadsPlanet
{
    // 차 속도는 최대 30km/h, 15km/h부터 줄이고, 거리가 10되면 멈추기
    // 현재 도로를 가는데, 현재 도로의 정보를 얻어오고, 좌회전할지 우회전할지, 직진할 지 선택할 수 있게 하기
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
            if (other.TryGetComponent<RoadController>(out var roadController))
            {
                Debug.Log(roadController.LaneType.ToString());
            }

            // 
            if (other.TryGetComponent<TrafficFlow>(out var trafficFlow))
            {
                Debug.Log(trafficFlow.currentLightType.ToString());
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
                Debug.Log("Player Detected! Distance: " + hit.distance);

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
                Debug.Log("Too Close! Slowing down or stopping...");
                // 여기에 차량의 속도를 조절하는 코드를 작성하세요.
            }
        }

        private void CarMovement()
        {
            // todo: 점진적으로 0~1로 갈 수 있도록 하기
            _carController.Move(-.05f, .1f, .1f, 0);
        }

        // 직진
        public void GoThrough()
        {
            Debug.Log("직전");
        }

        // 유턴
        public void UTurn()
        {
            Debug.Log("유턴");
        }

        // 차선 변경
        public void ChangeLane()
        {
            Debug.Log("차선 변경");
        }

        // 좌회전
        public void TurnLeft()
        {
            Debug.Log("좌회전");
        }

        // 우회전
        public void TurnRight()
        {
            Debug.Log("우회전");
        }
    }
}