using System;
using NomadsPlanet.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace NomadsPlanet
{
    public class CarDetector : MonoBehaviour
    {
        // 앞으로 올 친구의 정보
        [ShowInInspector] public CarHandler TargetCar { get; set; } = CarHandler.NullCar;
        public LaneType ThisLane { get; private set; }
        public int Index { get; private set; }

        // 차량 들어왔을 때 진행시킬 이벤트
        private UnityAction<CarHandler> _carEnterEvent;

        public void InitSetup(LaneType lane, int idx, UnityAction<CarHandler> carEnterEvent)
        {
            ThisLane = lane;
            Index = idx;
            _carEnterEvent = carEnterEvent;
        }

        public bool TargetCarOnThisPoint()
        {
            if (TargetCar == CarHandler.NullCar)
            {
                return false;
            }

            return Vector3.Distance(TargetCar.transform.position, transform.position) < 2f;
        }

        public bool CarOnThisPoint(CarHandler car)
        {
            return Vector3.Distance(car.transform.position, transform.position) < 1f;
        }

        // update? - 무시되는 영역이 생기기도 하네
        private void OnTriggerStay(Collider other)
        {
            if (TargetCar != null && other.gameObject.layer != 6 ||
                !other.TryGetComponent<CarColliderGetter>(out var car))
            {
                return;
            }

            if (Index == 0)
            {
                return;
            }

            if (car.CarHandler.IsMoving)
            {
                return;
            }

            _carEnterEvent(car.CarHandler);
        }
    }
}