using UnityEngine;
using UnityEngine.Events;

namespace NomadsPlanet
{
    public class CarDetector : MonoBehaviour
    {
        private UnityAction<CarHandler> _carEnterEvent;
        
        public void InitSetup(UnityAction<CarHandler> carEnterEvent)
        {
            _carEnterEvent = carEnterEvent;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<CarHandler>(out var car))
            {
                _carEnterEvent(car);
            }
        }
    }
}