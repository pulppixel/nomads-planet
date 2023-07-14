using System;
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

        [ShowInInspector, ReadOnly]
        private Transform _leftWayPoint; // 왼쪽 좌회전 경유지, 곡선 이동 수학 지식 미숙

        [SerializeField, RequiredListLength(2)]
        private Transform[] rightCarTargets = new Transform[2];

        [ShowInInspector, ReadOnly]
        private Transform _rightWayPoint; // 우회전 경유지

        [InfoBox("현재 차선에서 차량이 위치할 수 있는 곳들 목록")]
        [ShowInInspector, ReadOnly]
        public List<Transform> LeftCarPoints { get; private set; } // 0번 인덱스가 가장 앞에 위치함

        private List<bool> _leftCarPlaced = new();

        [ShowInInspector, ReadOnly]
        public List<Transform> RightCarPoints { get; private set; }

        private List<bool> _rightCarPlaced = new();

        private TrafficType _thisTrafficType;
        private LightType _curLightType;

        private LightController _lightController;
        private CarDetector _carDetector;

        private void Awake()
        {
            _InitGetters();
        }

        // Index 0번에 도착했을 때 발생시킬 액션도 있어야해
        private void Update()
        {
            CarOnFirstLine();
        }

        private const int MaxCount = 2;
        private int _yellowCount;

        private void CarOnFirstLine()
        {
            // 차가 없을 때는 동작 x
            if (_carDetector.GetCarLength().Equals(0))
            {
                return;
            }

            var leftCar = _carDetector.GetCarOnPosition(LeftCarPoints[0]);
            var rightCar = _carDetector.GetCarOnPosition(rightCarTargets[0]);

            if (leftCar == CarHandler.NullCar && rightCar == CarHandler.NullCar)
            {
                return;
            }

            switch (_curLightType)
            {
                case LightType.Red:
                    _yellowCount = 0;
                    return;
                case LightType.Yellow when _yellowCount++ >= MaxCount:
                    return;
                case LightType.Green:
                default:
                    break;
            }

            if (leftCar != CarHandler.NullCar)
            {
                _carDetector.ExitFirstCar(leftCar);
            }

            if (rightCar != CarHandler.NullCar)
            {
                _carDetector.ExitFirstCar(rightCar);
            }

            // 나머지 애들도 재배열
        }

        private void OnCarEnterEvent(CarHandler insideCar)
        {
            // 처음 차가 들어갔을 때, 향해야할 목표 지점을 정해준다.
            var targetTrafficType = _thisTrafficType.GetRandomTrafficType();
            insideCar.SetTrafficTarget(targetTrafficType);
        }


        private void OnCarExitEvent(CarHandler insideCars)
        {
            // 차가 나갔을 때, 나머지 친구들이 앞 공간을 채워줄 수 있도록 하자.
        }

        private void ArrangeAllCars()
        {
            switch (_thisTrafficType)
            {
                case TrafficType.Left:
                    // 차선 두 개다 왼쪽으로 도는 차선.
                    break;
                case TrafficType.Right:
                    break;
                case TrafficType.Left | TrafficType.Right:
                    break;
                case TrafficType.Forward | TrafficType.Left | TrafficType.Right:
                    break;
                case TrafficType.Forward:
                default:
                    break;
            }
            
            
            // 왼쪽으로 설정된 놈이면 왼쪽 차선으로 보내주기
            
            // 오른쪽으로 설정된 놈이면 오른쪽 차선으로 보내주기
            
            // 직진으로 설정된 놈이면..... 사실 어디든 상관 없어.
            
            // todo 주의: 왼쪽으로만 설정되었을 수도 있다. 이 조건을 가장 처음으로 정해야 함 
        }

        // LightController에서 횡단보도 타입을 정할 수 있도록 해준다.
        public void SetLightAction(LightType type)
        {
            _curLightType = type;
            _lightController.SetTrafficSign(type);
        }

        // 여기서 필요한 멤버들을 초기화해준다.
        private void _InitGetters()
        {
            _carDetector = GetComponent<CarDetector>();
            _thisTrafficType = TrafficManager.GetTrafficType(tag);
            _carDetector.InitSetup(OnCarEnterEvent, OnCarExitEvent);
            _lightController = transform.GetChildFromName<LightController>("TrafficLight");

            var leftParents = transform.GetChildFromName<Transform>("1");
            var rightParents = transform.GetChildFromName<Transform>("2");
            _leftWayPoint = leftParents.GetChildFromName<Transform>("waypoint") ?? transform;
            _rightWayPoint = rightParents.GetChildFromName<Transform>("waypoint") ?? transform;

            LeftCarPoints = new List<Transform>();
            RightCarPoints = new List<Transform>();
            for (int i = 0; i < leftParents.childCount - 1; i++)
            {
                LeftCarPoints.Add(leftParents.GetChild(i));
                _leftCarPlaced.Add(false);
            }

            for (int i = 0; i < rightParents.childCount - 1; i++)
            {
                RightCarPoints.Add(rightParents.GetChild(i));
                _rightCarPlaced.Add(false);
            }
        }
    }
}