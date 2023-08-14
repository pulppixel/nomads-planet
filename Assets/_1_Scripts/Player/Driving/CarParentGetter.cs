using UnityEngine;

namespace NomadsPlanet
{
    public class CarParentGetter : MonoBehaviour
    {
        public PlayerScore PlayerScore { get; private set; }

        private void Awake()
        {
            var parentObj = transform.parent.parent;
            PlayerScore = parentObj.GetComponent<PlayerScore>();
        }
    }
}