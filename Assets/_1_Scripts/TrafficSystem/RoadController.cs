using NomadsPlanet.Utils;
using UnityEngine;

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