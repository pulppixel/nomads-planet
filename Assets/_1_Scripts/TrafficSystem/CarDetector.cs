using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NomadsPlanet
{
    public class CarDetector : MonoBehaviour
    {
        // 차가 앞으로 갈 곳은 본인이 정할 거야
        [ShowInInspector, ReadOnly]
        private List<CarHandler> _insideCars = new();

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 6 && other.TryGetComponent<CarColliderGetter>(out var car))
            {
                _insideCars.Add(car.CarHandler);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == 6 && other.TryGetComponent<CarColliderGetter>(out var car))
            {
                _insideCars.Remove(car.CarHandler);
            }
        }
    }
}