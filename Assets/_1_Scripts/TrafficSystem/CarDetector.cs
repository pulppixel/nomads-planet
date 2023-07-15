using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace NomadsPlanet
{
    public class CarDetector : MonoBehaviour
    {
        // FIFO
        [ShowInInspector, ReadOnly]
        private List<CarHandler> _insideCars = new();

        private UnityAction<CarHandler> _carEntranceMove; // 이거 존나 두루뭉실함
        private UnityAction<CarHandler> _carExitMove;

        // Required
        public void InitSetup(UnityAction<CarHandler> newCarEntered, UnityAction<CarHandler> newCarExited)
        {
            _carEntranceMove = newCarEntered;
            _carExitMove = newCarExited;
        }

        // Event Function (Push)
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != 6 || !other.TryGetComponent<CarColliderGetter>(out var car))
            {
                return;
            }

            _insideCars.Add(car.CarHandler);
            _carEntranceMove.Invoke(car.CarHandler);
        }

        // Not Event Function (Pop)
        public void ExitFirstCar(CarHandler car)
        {
            if (car == CarHandler.NullCar)
            {
                return;
            }

            _carExitMove.Invoke(car);
            _insideCars.Remove(car); // 근데 꼭 0번이 맨 앞이 아닐 수도 있어.
            
            // 어레인지 여기서 할까?
        }

        // 현재 차량의 총 개수를 받아온다.
        public int GetCarLength() => _insideCars.Count;

        public CarHandler GetCarOnPosition(Transform target) =>
            _insideCars.FirstOrDefault(car => Vector3.Distance(target.position, car.transform.position) < 1) ?? CarHandler.NullCar;
    }
}