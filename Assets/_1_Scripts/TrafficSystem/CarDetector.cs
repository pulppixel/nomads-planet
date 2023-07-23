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
        [ShowInInspector]
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

            return Vector3.Distance(TargetCat.transform.position, transform.position) < 2f;
        }

        // update? - 무시되는 영역이 생기기도 하네
        private void OnTriggerStay(Collider other)
        {
            if (TargetCat != null && other.gameObject.layer != 6 || !other.TryGetComponent<CarColliderGetter>(out var car))
            {
                return;
            }

            _carEnterEvent(car.CarHandler);
        }
    }
}