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
        // "해당 차선에서 갈 수 있는 곳들 목록"
        [SerializeField, RequiredListLength(2)]
        private Transform[] leftCarTargets = new Transform[2]; // 좌회전, 직진

        private Transform _leftWayPoint; // 왼쪽 좌회전 경유지, 곡선 이동 수학 지식 미숙

        [SerializeField, RequiredListLength(2)]
        private Transform[] rightCarTargets = new Transform[2]; // 우회전, 직진

        private Transform _rightWayPoint; // 우회전 경유지

        // "현재 차선에서 차량이 위치할 수 있는 곳들 목록"
        private List<CarDetector> LeftCarDetectors { get; set; } // 0번 인덱스가 가장 앞에 위치함

        private readonly List<bool> _leftCarPlaced = new(6);

        private List<CarDetector> RightCarDetectors { get; set; } // 차량이 이동할 포지션들이다.

        private readonly List<bool> _rightCarPlaced = new(6);

        // 현재 내부에 들어있는 차량
        private readonly List<CarHandler> _insideCars = new(12);

        private TrafficType _thisTrafficType;
        private LightType _curLightType;

        private LightController _lightController;

        private const int MaxCount = 2;
        private int _yellowCount;

        private void Awake() => _InitGetters();

        // Index 0번에 도착했을 때 발생시킬 액션도 있어야해
        private void FixedUpdate() => _CarOnFirstLine();

        // LightController에서 횡단보도 타입을 정할 수 있도록 해준다.
        public void SetLightAction(LightType type)
        {
            _curLightType = type;
            _lightController.SetTrafficSign(type);
        }

        private void _CarOnFirstLine()
        {
            // 빨간 불일 때는 차를 그대로 둬야 한다.
            if (_curLightType == LightType.Red)
            {
                _yellowCount = 0;
                return;
            }

            // 만약 노란불이거나, 노란불인데, 차량 2대가 지나간 상황이라면, 나머지 차들도 그대로 둬야 한다.
            if (_curLightType == LightType.Yellow && _yellowCount++ >= MaxCount)
            {
                return;
            }

            // 차가 없을 때는 동작 x
            bool hasCarOnRoad = _insideCars.Count > 0;
            if (!hasCarOnRoad)
            {
                return;
            }

            // 왼쪽이나 오른쪽에서 차가 젤 앞에 도달했는지 확인한다.
            var leftCar = LeftCarDetectors[0].GetThisCar;
            var rightCar = RightCarDetectors[0].GetThisCar;

            bool ifCarOnForward = leftCar != CarHandler.NullCar || rightCar != CarHandler.NullCar;
            if (!ifCarOnForward)
            {
                return;
            }

            // 만약 왼쪽차가 가장 앞에 정상적으로 도달했다면,
            if (leftCar != CarHandler.NullCar && _insideCars.Contains(leftCar))
            {
                // 왼쪽 차를 출발시킨다. (OnCarExitEvent 가 발생된다.)
                _OnCarExitMove(leftCar);
            }

            // 그리고 오른쪽 차도 가장 앞에 정상적으로 도달했다면,
            if (rightCar != CarHandler.NullCar && _insideCars.Contains(rightCar))
            {
                // 오른쪽 차도 출발시킨다.
                _OnCarExitMove(rightCar);
            }
        }

        // 6번 위치에 들어왔을 때.. 이동할 수 있도록 이벤트로 등록한다.
        private void _OnCarEntranceMove(CarHandler insideCar)
        {
            // 이미 들어와있는 애면, 또 옮겨줄 필요가 없다.
            if (_insideCars.Contains(insideCar))
            {
                return;
            }

            // 처음 차가 들어갔을 때, 향해야할 목표 지점을 정해준다.
            var targetTrafficType = _thisTrafficType.GetRandomTrafficType();
            insideCar.SetTrafficTarget(targetTrafficType);

            // 타겟 포지션. 해당 위치로 이동할 수 있도록 한다.
            Transform targetPoint;
            LaneType carLaneType;

            // 각 방향 별로 상태 목표 지점 정하기
            switch (targetTrafficType)
            {
                case TrafficType.Left:
                    targetPoint = _GetLeftEmptyPoint();
                    carLaneType = LaneType.First; // 1차로
                    break;
                    
                    // 왼쪽 차선으로 가기. 그런데, 여기가 왼쪽밖에 없는 곳이라면?
                    bool ifOnlyLeft = !_thisTrafficType.HasFlag(TrafficType.Forward) &&
                                      !_thisTrafficType.HasFlag(TrafficType.Right);

                    if (!ifOnlyLeft)
                    {
                        targetPoint = _GetLeftEmptyPoint();
                        carLaneType = LaneType.First; // 1차로
                        break;
                    }

                    bool isLeft = Random.Range(0f, 1f) < 0.5f;

                    // 왼쪽만 갈 수 있는 곳이라면, 양쪽 차선 전부 이용해도 큰 문제는 없다.
                    carLaneType = isLeft ? LaneType.First : LaneType.Second;
                    targetPoint = isLeft ? _GetLeftEmptyPoint() : _GetRightEmptyPoint();
                    break;
                case TrafficType.Right:
                    targetPoint = _GetRightEmptyPoint();
                    carLaneType = LaneType.Second;
                    break;
                    
                    bool ifOnlyRight = !_thisTrafficType.HasFlag(TrafficType.Forward) &&
                                       !_thisTrafficType.HasFlag(TrafficType.Left);
                    if (ifOnlyRight)
                    {
                        targetPoint = _GetRightEmptyPoint();
                        carLaneType = LaneType.Second;
                        break;
                    }

                    bool isRight = Random.Range(0f, 1f) < 0.5f;

                    carLaneType = isRight ? LaneType.Second : LaneType.First;
                    targetPoint = isRight ? _GetLeftEmptyPoint() : _GetRightEmptyPoint();
                    break;
                case TrafficType.Forward:
                default:
                    // * 직진이 가능한 이상, 어느 차선이던 상관 없다.
                    bool getLeftOrRight = Random.value < 0.5f;
                    carLaneType = getLeftOrRight ? LaneType.First : LaneType.Second;
                    targetPoint = getLeftOrRight ? _GetLeftEmptyPoint() : _GetRightEmptyPoint();
                    break;
            }

            _insideCars.Add(insideCar);

            // 최종적으로 결정된 해당 위치까지 차량을 출발시킨다.
            insideCar.MoveToTarget(targetPoint ? targetPoint : insideCar.transform, carLaneType);
        }

        private void _OnCarExitMove(CarHandler insideCar)
        {
            if (insideCar.IsMoving)
            {
                return;
            }

            switch (insideCar.TargetType)
            {
                case TrafficType.Left:
                    // 왼쪽 경유지를 들른 후, 다음으로 넘어갈 수 있도록 한다.
                    insideCar.MoveViaWaypoint(leftCarTargets[0], _leftWayPoint);
                    _leftCarPlaced[0] = false;
                    break;
                case TrafficType.Right:
                    insideCar.MoveViaWaypoint(rightCarTargets[0], _rightWayPoint);
                    _rightCarPlaced[0] = false;
                    break;
                case TrafficType.Forward:
                default:
                    bool isLeft = LeftCarDetectors[0].GetThisCar == insideCar;
                    if (isLeft)
                    {
                        insideCar.MoveToTarget(leftCarTargets[1], LaneType.First);
                        _leftCarPlaced[0] = false;
                        return;
                    }

                    bool isRight = RightCarDetectors[0].GetThisCar == insideCar;
                    if (isRight)
                    {
                        insideCar.MoveToTarget(rightCarTargets[1], LaneType.Second);
                        _rightCarPlaced[0] = false;
                    }

                    break;
            }

            _insideCars.Remove(insideCar);
            _ArrangeCars();
        }

        private void _ArrangeCars()
        {
            // 왼쪽 차선 처리
            _ProcessLane(_leftCarPlaced, _insideCars.Where(car => car.CurLaneType == LaneType.First).ToList(),
                leftCarTargets);

            // 오른쪽 차선 처리
            _ProcessLane(_rightCarPlaced, _insideCars.Where(car => car.CurLaneType == LaneType.Second).ToList(),
                rightCarTargets);
        }

        private void _ProcessLane(IList<bool> lane, IReadOnlyList<CarHandler> cars, IReadOnlyList<Transform> targets)
        {
            List<int> emptyIndices = new();
            List<int> carIndices = new();
            for (int i = 0; i < lane.Count; i++)
            {
                if (lane[i])
                {
                    carIndices.Add(i);
                }
                else
                {
                    emptyIndices.Add(i);
                }
            }

            foreach (var index in emptyIndices)
            {
                if (carIndices.Count == 0 || index > carIndices[0])
                {
                    break;
                }

                int carIndex = carIndices[0];
                carIndices.RemoveAt(0);

                lane[index] = true;
                lane[carIndex] = false;

                cars[carIndex].MoveToTarget(targets[index], cars[carIndex].CurLaneType);
            }
        }

        private Transform _GetLeftEmptyPoint()
        {
            var detector = (
                from t in LeftCarDetectors
                where t.GetThisCar == CarHandler.NullCar
                select t
            ).FirstOrDefault();

            _leftCarPlaced[detector!.ThisIndex] = true;

            return detector.GetComponent<Transform>();
        }

        private Transform _GetRightEmptyPoint()
        {
            var detector = (
                from t in RightCarDetectors
                where t.GetThisCar == CarHandler.NullCar
                select t
            ).FirstOrDefault();

            _rightCarPlaced[detector!.ThisIndex] = true;

            return detector.GetComponent<Transform>();
        }

        // 여기서 필요한 멤버들을 초기화해준다.
        private void _InitGetters()
        {
            _thisTrafficType = TrafficManager.GetTrafficType(tag);
            _lightController = transform.GetChildFromName<LightController>("TrafficLight");

            var leftParents = transform.GetChildFromName<Transform>("1");
            var rightParents = transform.GetChildFromName<Transform>("2");

            _leftWayPoint = leftParents.GetChildFromName<Transform>("waypoint") ?? transform;
            _rightWayPoint = rightParents.GetChildFromName<Transform>("waypoint") ?? transform;

            LeftCarDetectors = new List<CarDetector>(6);
            RightCarDetectors = new List<CarDetector>(6);
            for (int i = 0; i < leftParents.childCount; i++)
            {
                bool isDetector = leftParents.GetChild(i).TryGetComponent<CarDetector>(out var detector);
                if (!isDetector)
                {
                    continue;
                }

                detector.InitSetup(LaneType.First, i, _OnCarEntranceMove);
                LeftCarDetectors.Add(detector);
                _leftCarPlaced.Add(false);
            }

            for (int i = 0; i < rightParents.childCount; i++)
            {
                bool isDetector = rightParents.GetChild(i).TryGetComponent<CarDetector>(out var detector);
                if (!isDetector)
                {
                    continue;
                }

                detector.InitSetup(LaneType.Second, i, _OnCarEntranceMove);
                RightCarDetectors.Add(detector);
                _rightCarPlaced.Add(false);
            }
        }
    }
}