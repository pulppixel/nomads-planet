using UnityEngine;
using NomadsPlanet.Utils;

namespace NomadsPlanet
{
    public class RoadController : MonoBehaviour
    {
        public LaneType LaneType { get; private set; }
        
        private void Awake()
        {
            LaneType = TrafficManager.GetLaneType(name);
        }
    }
}