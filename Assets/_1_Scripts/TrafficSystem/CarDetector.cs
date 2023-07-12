using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace NomadsPlanet
{
    public class CarDetector : MonoBehaviour
    {
        // 차가 앞으로 갈 곳은 본인이 정할 거야
        [ShowInInspector, ReadOnly] private List<CarHandler> _insideCars = new();

        private UnityAction<CarHandler> _newCarEntranced;
        private UnityAction<CarHandler> _newCarExited;

        // Required
        public void InitSetup(UnityAction<CarHandler> newCarEntered, UnityAction<CarHandler> newCarExited)
        {
            _newCarEntranced = newCarEntered;
            _newCarExited = newCarExited;
        }

        // 현재 차량의 총 개수를 받아온다.
        public int GetCarLength() => _insideCars.Count;

        public bool GetCarOnPosition(Transform target)
        {
            return _insideCars.Any(car => Vector3.Distance(target.position, car.transform.position) < 1);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != 6 || !other.TryGetComponent<CarColliderGetter>(out var car))
            {
                return;
            }

            _insideCars.Add(car.CarHandler);
            _newCarEntranced.Invoke(car.CarHandler);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer != 6 || !other.TryGetComponent<CarColliderGetter>(out var car))
            {
                return;
            }

            _insideCars.Remove(car.CarHandler);
            _newCarExited.Invoke(car.CarHandler);
        }
    }
}