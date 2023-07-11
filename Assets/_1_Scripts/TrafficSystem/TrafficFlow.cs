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

        [ShowInInspector, ReadOnly]
        public List<Transform> rightCarPoints { get; private set; }
        
        public TrafficType trafficType { get; private set; }
        public LightType curLightType { get; private set; }
        
        private LightController _lightController;
        private CarDetector _carDetector;

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

        /* CAR STATE를 정해보자. (현재 신호등의 색상에 따라 내용 정리를 해야한다.)
         * 1. 처음에 들어왔을 때, 위치할 곳을 정해준다. (50% 확률로 왼쪽, 오른쪽 차선 구분),
         *    차선 정해주는 것부터 해서 전체 이동은... 여기서 할듯?
         *    대신 이동 관련 내용은 `CarHandler`에 넣어야함
         * 2. 맨 앞에 있는 애 또한 분기점이 있을 시, 50% 확률로 갈 곳 정하기
         * 3. 노란불은 멈추는 불. 무리하게 건너진 말고, 보이면 멈추되, 앞 2개까지는 건널 수 있게 하기
         * 4. 
         */ 

        // 여기서 필요한 멤버들을 초기화해준다.
        private void _InitGetters()
        {
            trafficType = TrafficManager.GetTrafficType(tag);
            _carDetector = GetComponent<CarDetector>();
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