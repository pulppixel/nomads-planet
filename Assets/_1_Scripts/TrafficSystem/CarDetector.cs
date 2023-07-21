using System;
using NomadsPlanet.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace NomadsPlanet
{
    public class CarDetector : MonoBehaviour
    {
        // 앞으로 올 친구의 정보
        public CarHandler TargetCat { get; set; } = CarHandler.NullCar;
        public LaneType ThisLane { get; private set; }

        // 차량 들어왔을 때 진행시킬 이벤트
        private UnityAction<CarHandler> _carEnterEvent;

        public void InitSetup(LaneType lane, UnityAction<CarHandler> carEnterEvent)
        {
            ThisLane = lane;
            _carEnterEvent = carEnterEvent;
        }

        public bool CarOnThisPoint()
        {
            if (TargetCat == CarHandler.NullCar)
            {
                return false;
            }

            return Vector3.Distance(TargetCat.transform.position, this.transform.position) < 1f;
        }

        // update?
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != 6 || !other.TryGetComponent<CarColliderGetter>(out var car))
            {
                return;
            }

            _carEnterEvent(car.CarHandler);
        }
    }
}