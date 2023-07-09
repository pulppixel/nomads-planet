using System.Collections.Generic;
using UnityEngine;
using NomadsPlanet.Utils;
using LightType = NomadsPlanet.Utils.LightType;

namespace NomadsPlanet
{
    // 현재 어떤 신호를 갖고 있는지 알려준다.
    // 좌회전이나 우회전만 가능한 차선에서는, 2차선 모두 이용이 가능하다.
    public class TrafficFlow : MonoBehaviour
    {
        // 여기서 갈 수 있는 곳들 (각 하나씩)
        // 차량이 이동하고 싶어하는 방향 받아오기
        // 차량 이동시키기 (World 좌표계로 이동시켜도 되고 뭐.
        public Transform leftCarParent;
        public Transform rightCarParent;
        
        private List<Transform> _leftCarPoints;  
        private List<Transform> _rightCarPoints;  
        
        
        
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