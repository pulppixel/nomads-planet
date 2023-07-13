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

        private UnityAction<CarHandler> _newCarEntranced;
        private UnityAction<CarHandler> _newCarExited;

        // Required
        public void InitSetup(UnityAction<CarHandler> newCarEntered, UnityAction<CarHandler> newCarExited)
        {
            _newCarEntranced = newCarEntered;
            _newCarExited = newCarExited;
        }
        
        // Event Function (Push)
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != 6 || !other.TryGetComponent<CarColliderGetter>(out var car))
            {
                return;
            }

            _insideCars.Add(car.CarHandler);
            _newCarEntranced.Invoke(car.CarHandler);
        }
        
        // Not Event Function (Pop)
        public void ExitFirstCar()
        {
            _newCarExited.Invoke(_insideCars.First());
            _insideCars.RemoveAt(0);
        }

        // 현재 차량의 총 개수를 받아온다.
        public int GetCarLength() => _insideCars.Count;

        public bool GetCarOnPosition(Transform target)
        {
            return _insideCars.Any(car => Vector3.Distance(target.position, car.transform.position) < 1);
        }
        
    }
}