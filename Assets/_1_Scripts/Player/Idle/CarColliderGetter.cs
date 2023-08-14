using UnityEngine;
using Sirenix.OdinInspector;

namespace NomadsPlanet
{
    public class CarColliderGetter : MonoBehaviour
    {
        [ShowInInspector, ReadOnly]
        public CarHandler CarHandler { get; private set; }

        private void Awake()
        {
            var parentObj = transform.parent.parent;
            CarHandler = parentObj.GetComponent<CarHandler>();
        }
    }
}