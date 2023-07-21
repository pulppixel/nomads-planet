using System.Linq;
using UnityEngine;
using NomadsPlanet.Utils;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using LightType = NomadsPlanet.Utils.LightType;

namespace NomadsPlanet
{
    // 현재 어떤 신호를 갖고 있는지 알려준다.
    // 좌회전이나 우회전만 가능한 차선에서는, 2차선 모두 이용이 가능하다.
    public class TrafficFlow : MonoBehaviour
    {
        public float rotateY;

        // "해당 차선에서 갈 수 있는 곳들 목록"
        [SerializeField, RequiredListLength(2)]
        private Transform[] leftCarTargets = new Transform[2]; // 좌회전, 직진

        private Transform[] _leftWayPoint = new Transform[2]; // 왼쪽 좌회전 경유지

        [SerializeField, RequiredListLength(2)]
        private Transform[] rightCarTargets = new Transform[2]; // 우회전, 직진

        private Transform[] _rightWayPoint = new Transform[2]; // 우회전 경유지

        // "현재 차선에서 차량이 위치할 수 있는 곳들 목록"
        private List<CarDetector> LeftCarDetectors { get; set; }
        private List<CarDetector> RightCarDetectors { get; set; }

        [ShowInInspector] private List<CarHandler> _insideCars = new(14);

        // 이 아래는 클래스 성질을 나타내기 위함. 볼 필요 x
        private LightType _curLightType;
        private TrafficType _thisTrafficType;
        private LightController _lightController;

        private void Awake() => _Init();

        private void FixedUpdate() => _OnCarUpdate();

        // 차량이 들어왔다면,
        private void OnCarEnter(CarHandler car)
        {
            if (_insideCars.Contains(car))
            {
                return;
            }

            _insideCars.Add(car);

            // 맨 앞에서부터 탐색하고, 빈 곳이 있으면 거기로 보내
            // 한번 정해지면, 거기로만가
            for (int i = 0; i < LeftCarDetectors.Count; i++)
            {
                if (Random.value < .5f)
                {
                    if (LeftCarDetectors[i].TargetCat == CarHandler.NullCar)
                    {
                        LeftCarDetectors[i].TargetCat = car;
                        car.MoveToTarget(LeftCarDetectors[i].transform.position);
                        break;
                    }
                }
                else
                {
                    if (RightCarDetectors[i].TargetCat == CarHandler.NullCar)
                    {
                        RightCarDetectors[i].TargetCat = car;
                        car.MoveToTarget(RightCarDetectors[i].transform.position);
                        break;
                    }
                }
            }
        }

        private void _OnCarUpdate()
        {
            if (_curLightType is LightType.Red or LightType.Yellow)
            {
                return;
            }

            // 맨 앞에 놈은 아예 다른 곳으로 가게 하기
            bool isLeftOnCar = LeftCarDetectors[0].TargetCat != CarHandler.NullCar &&
                               LeftCarDetectors[0].CarOnThisPoint() &&
                               _insideCars.Contains(LeftCarDetectors[0].TargetCat);

            if (isLeftOnCar)
            {
                bool isLeft = _thisTrafficType.HasFlag(TrafficType.Left) || _thisTrafficType.HasFlag(TrafficType.Right);
                bool isForward = _thisTrafficType.HasFlag(TrafficType.Forward);

                _insideCars.Remove(LeftCarDetectors[0].TargetCat);
                MoveToOtherLane(LeftCarDetectors, leftCarTargets, _leftWayPoint, isLeft, isForward);
            }

            bool isRightOnCar = RightCarDetectors[0].TargetCat != CarHandler.NullCar &&
                                RightCarDetectors[0].CarOnThisPoint() &&
                                _insideCars.Contains(RightCarDetectors[0].TargetCat);

            if (isRightOnCar)
            {
                bool isRight = _thisTrafficType.HasFlag(TrafficType.Right) || _thisTrafficType.HasFlag(TrafficType.Left);
                bool isForward = _thisTrafficType.HasFlag(TrafficType.Forward);

                _insideCars.Remove(RightCarDetectors[0].TargetCat);
                MoveToOtherLane(RightCarDetectors, rightCarTargets, _rightWayPoint, isRight, isForward);
            }

            void MoveToOtherLane(IReadOnlyList<CarDetector> carDetectors, IReadOnlyList<Transform> carTargets,
                IReadOnlyList<Transform> wayPoints, bool isCurved, bool isForward)
            {
                if (isCurved && isForward)
                {
                    if (Random.value < .5f)
                    {
                        carDetectors[0].TargetCat.MoveViaWaypoint(carTargets[0].position,
                            new[] { wayPoints[0].position, wayPoints[1].position });
                    }
                    else
                    {
                        carDetectors[0].TargetCat.MoveToTarget(carTargets[1].position);
                    }
                }
                else if (isCurved)
                {
                    carDetectors[0].TargetCat.MoveViaWaypoint(carTargets[0].position,
                        new[] { wayPoints[0].position, wayPoints[1].position });
                }
                else if (isForward)
                {
                    carDetectors[0].TargetCat.MoveToTarget(carTargets[1].position);
                }

                carDetectors[0].TargetCat = CarHandler.NullCar;

                for (int i = 1; i < carDetectors.Count; i++)
                {
                    if (carDetectors[i].TargetCat != CarHandler.NullCar && carDetectors[i].CarOnThisPoint())
                    {
                        carDetectors[i].TargetCat.MoveToTarget(carDetectors[i - 1].transform.position);
                        carDetectors[i - 1].TargetCat = carDetectors[i].TargetCat;
                        carDetectors[i].TargetCat = CarHandler.NullCar;
                    }
                }
            }
        }

        private void _Init()
        {
            _thisTrafficType = TrafficManager.GetTrafficType(tag);
            _lightController = transform.GetChildFromName<LightController>("TrafficLight");

            var leftParents = transform.GetChildFromName<Transform>("1");
            var rightParents = transform.GetChildFromName<Transform>("2");

            _leftWayPoint[0] = leftParents.GetChildFromName<Transform>("waypoint") ?? transform;
            _rightWayPoint[0] = rightParents.GetChildFromName<Transform>("waypoint") ?? transform;
            _leftWayPoint[1] = leftParents.GetChildFromName<Transform>("waypoint (1)") ?? transform;
            _rightWayPoint[1] = rightParents.GetChildFromName<Transform>("waypoint (1)") ?? transform;

            LeftCarDetectors = new(7);
            RightCarDetectors = new(7);

            for (int i = 0; i < leftParents.childCount; i++)
            {
                bool isDetector = leftParents.GetChild(i).TryGetComponent<CarDetector>(out var detector);
                if (!isDetector)
                {
                    continue;
                }

                detector.InitSetup(LaneType.First, OnCarEnter);
                LeftCarDetectors.Add(detector);
            }

            for (int i = 0; i < rightParents.childCount; i++)
            {
                bool isDetector = rightParents.GetChild(i).TryGetComponent<CarDetector>(out var detector);
                if (!isDetector)
                {
                    continue;
                }

                detector.InitSetup(LaneType.Second, OnCarEnter);
                RightCarDetectors.Add(detector);
            }
        }


        // 외부에서 쓸 애들
        public List<Transform> GetTargetValues()
        {
            List<Transform> targets = new List<Transform>(12);
            targets.AddRange(LeftCarDetectors.Select(obj => obj.transform));
            targets.AddRange(RightCarDetectors.Select(obj => obj.transform));

            return targets;
        }

        // LightController에서 횡단보도 타입을 정할 수 있도록 해준다.
        public void SetLightAction(LightType type)
        {
            _curLightType = type;
            _lightController.SetTrafficSign(type);
        }
    }
}