using UnityEngine;
using NomadsPlanet.Utils;

namespace NomadsPlanet
{
    // Lane Type 없애고.. 타일 별로 계산할 수 있도록
    public class LaneController : MonoBehaviour
    {
        public LaneType LaneType { get; private set; }
        
        private void Awake()
        {
            LaneType = TrafficManager.GetLaneType(name);
        }
    }
}