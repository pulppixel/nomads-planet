using System;
using NomadsPlanet.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace NomadsPlanet
{
    public class CarDetector : MonoBehaviour
    {
        // 현재 올라와있는 차의 정보를 전할 수 있다.
        public CarHandler GetThisCar { get; private set; } = CarHandler.NullCar;
        public LaneType ThisLane { get; private set; }
        public int ThisIndex { get; private set; }

        // 차량 들어왔을 때 진행시킬 이벤트
        private UnityAction<CarHandler> _carEnterEvent;

        public void InitSetup(LaneType lane, int idx, UnityAction<CarHandler> carEnterEvent)
        {
            ThisLane = lane;
            ThisIndex = idx;
            _carEnterEvent = carEnterEvent;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != 6 || !other.TryGetComponent<CarColliderGetter>(out var car))
            {
                return;
            }

            GetThisCar = car.CarHandler;
            _carEnterEvent(GetThisCar);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer != 6 || !other.TryGetComponent<CarColliderGetter>(out var car))
            {
                return;
            }

            if (car.CarHandler == GetThisCar)
            {
                GetThisCar = CarHandler.NullCar;
            }
        }
    }
}