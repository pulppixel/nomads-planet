using System;
using DG.Tweening;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

namespace NomadsPlanet
{
    // 차 속도는 최대 30km/h, 15km/h부터 줄이고, 거리가 10되면 멈추기
    // 현재 도로를 가는데, 현재 도로의 정보를 얻어오고, 좌회전할지 우회전할지, 직진할 지 선택할 수 있게 하기
    public class CarHandler : MonoBehaviour
    {
        public CarController carController;
        
        private const float MaxDistance = 20f;
        private const float SlowerDistance = 10f;
        
        public LayerMask layerMask;

        private void Update()
        {
            CarMovement();
            float distanceToForward = DetectOtherCar();
            if (Math.Abs(distanceToForward) > 0.001f)
            {
                // 만약 거리가 너무 가까우면 속도를 줄이거나 정지하게 합니다.
                if(distanceToForward < SlowerDistance)
                {
                    Debug.Log("Too Close! Slowing down or stopping...");
                    // 여기에 차량의 속도를 조절하는 코드를 작성하세요.
                }
            }
        }

        private float DetectOtherCar()
        {
            var tr = GetComponent<Transform>();
            RaycastHit hit;
            if (Physics.Raycast(tr.position, tr.forward, out hit, MaxDistance, layerMask))
            {
                Debug.Log("Player Detected! Distance: " + hit.distance);

                return hit.distance;
            }

            return 0f;
        }

        private void CarMovement()
        {
            carController.Move(0, 1, 1, 0);
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