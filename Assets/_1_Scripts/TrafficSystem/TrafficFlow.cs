using UnityEngine;
using NomadsPlanet.Utils;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using LightType = NomadsPlanet.Utils.LightType;

namespace NomadsPlanet
{
    // 현재 어떤 신호를 갖고 있는지 알려준다.
    // 좌회전이나 우회전만 가능한 차선에서는, 2차선 모두 이용이 가능하다.
    public class TrafficFlow : MonoBehaviour
    {
        [InfoBox("해당 차선에서 갈 수 있는 곳들 목록")]
        [SerializeField, RequiredListLength(2)]
        private Transform[] leftCarTargets = new Transform[2];

        [SerializeField, RequiredListLength(2)]
        private Transform[] rightCarTargets = new Transform[2];

        [InfoBox("현재 차선에서 차량이 위치할 수 있는 곳들 목록")]
        [ShowInInspector, ReadOnly]
        public List<Transform> leftCarPoints { get; private set; }

        [ShowInInspector, ReadOnly] public List<Transform> rightCarPoints { get; private set; }
        public TrafficType trafficType { get; private set; }
        public LightType curLightType { get; private set; }
        private LightController _lightController;

        private void Awake()
        {
            _InitGetters();
        }

        // LightController에서 횡단보도 타입을 정할 수 있도록 해준다.
        public void SetLightType(LightType type)
        {
            curLightType = type;
            _lightController.SetTrafficSign(type);
        }

        // 여기서 필요한 멤버들을 초기화해준다.
        private void _InitGetters()
        {
            trafficType = TrafficManager.GetTrafficType(tag);
            _lightController = transform.GetChild(0).GetComponent<LightController>();
            var leftParents = transform.GetChild(1);
            var rightParents = transform.GetChild(2);

            leftCarPoints = new List<Transform>();
            rightCarPoints = new List<Transform>();
            for (int i = 0; i < leftParents.childCount; i++)
            {
                leftCarPoints.Add(leftParents.GetChild(i));
            }

            for (int i = 0; i < rightParents.childCount; i++)
            {
                rightCarPoints.Add(rightParents.GetChild(i));
            }
        }
    }
}