using UnityEngine;
using NomadsPlanet.Utils;
using LightType = NomadsPlanet.Utils.LightType;

namespace NomadsPlanet
{
    // 현재 어떤 신호를 갖고 있는지 알려준다.
    // 좌회전이나 우회전만 가능한 차선에서는, 2차선 모두 이용이 가능하다.
    public class TrafficFlow : MonoBehaviour
    {
        public TrafficType trafficType { get; private set; }
        public LightType curLightType { get; private set; }
        private LightController _lightController;

        private void Awake()
        {
            trafficType = TrafficManager.GetTrafficType(tag);
            _lightController = transform.GetChild(0).GetComponent<LightController>();
        }

        public void SetLightType(LightType type)
        {
            curLightType = type;
            _lightController.SetTrafficSign(type);
        }
    }
}