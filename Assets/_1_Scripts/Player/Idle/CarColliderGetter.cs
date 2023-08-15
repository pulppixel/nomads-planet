using UnityEngine;

namespace NomadsPlanet
{
    public class CarColliderGetter : MonoBehaviour
    {
        public CarHandler CarHandler { get; private set; }

        private void Awake()
        {
            var parentObj = transform.parent.parent;
            CarHandler = parentObj.GetComponent<CarHandler>();
        }
    }
}