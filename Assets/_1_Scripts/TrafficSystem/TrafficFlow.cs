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
        [InfoBox("해당 차선에서 갈 수 있는 곳들 목록")] [SerializeField, RequiredListLength(2)]
        private Transform[] leftCarTargets = new Transform[2]; // 좌회전, 직진

        [ShowInInspector, ReadOnly] private Transform _leftWayPoint; // 왼쪽 좌회전 경유지, 곡선 이동 수학 지식 미숙

        [SerializeField, RequiredListLength(2)]
        private Transform[] rightCarTargets = new Transform[2]; // 우회전, 직진

        [ShowInInspector, ReadOnly] private Transform _rightWayPoint; // 우회전 경유지

        [InfoBox("현재 차선에서 차량이 위치할 수 있는 곳들 목록")]
        [ShowInInspector, ReadOnly]
        public List<Transform> LeftCarPoints { get; private set; } // 0번 인덱스가 가장 앞에 위치함

        private List<bool> _leftCarPlaced = new();

        [ShowInInspector, ReadOnly] public List<Transform> RightCarPoints { get; private set; }

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
            // 신호등 별 상황 부여
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

            // 차가 없을 때는 동작 x
            bool hasCarOnRoad = _carDetector.GetCarLength() > 0;
            if (!hasCarOnRoad)
            {
                return;
            }

            // 왼쪽이나 오른쪽에서 차가 젤 앞에 도달했는지 확인한다.
            CarHandler leftCar = _carDetector.GetCarOnPosition(LeftCarPoints[0]);
            CarHandler rightCar = _carDetector.GetCarOnPosition(RightCarPoints[0]);

            bool ifCarOnForward = leftCar != CarHandler.NullCar || rightCar != CarHandler.NullCar;
            if (!ifCarOnForward)
            {
                return;
            }

            // 만약 왼쪽차가 가장 앞에 정상적으로 도달했다면,
            if (leftCar != CarHandler.NullCar)
            {
                // 왼쪽 차를 출발시킨다. (OnCarExitEvent 가 발생된다.)
                _carDetector.ExitFirstCar(leftCar);
            }

            // 그리고 오른쪽 차도 가장 앞에 정상적으로 도달했다면,
            if (rightCar != CarHandler.NullCar)
            {
                // 오른쪽 차도 출발시킨다.
                _carDetector.ExitFirstCar(rightCar);
            }

            // 그리고 나머지 차들도 재배열해준다.
        }

        // # 코드 잘 짰다. 이건 FIX 해도 좋을듯
        private void OnCarEnterEvent(CarHandler insideCar)
        {
            // 처음 차가 들어갔을 때, 향해야할 목표 지점을 정해준다.
            var targetTrafficType = _thisTrafficType.GetRandomTrafficType();
            insideCar.SetTrafficTarget(targetTrafficType);
            
            // 타겟 포지션. 해당 위치로 이동할 수 있도록 한다.
            Transform targetPoint = null;

            // 각 방향 별로 상태 목표 지점 정하기
            switch (targetTrafficType)
            {
                case TrafficType.Left:
                    // 왼쪽 차선으로 가기. 그런데, 여기가 왼쪽밖에 없는 곳이라면?
                    bool ifOnlyLeft = !_thisTrafficType.HasFlag(TrafficType.Forward) &&
                                      !_thisTrafficType.HasFlag(TrafficType.Right);

                    if (!ifOnlyLeft)
                    {
                        targetPoint = GetLeftEmptyPoint();
                        break;
                    }

                    bool isLeft = Random.Range(0f, 1f) < 0.5f;

                    // 왼쪽만 갈 수 있는 곳이라면, 양쪽 차선 전부 이용해도 큰 문제는 없다.
                    targetPoint = isLeft ? GetLeftEmptyPoint() : GetRightEmptyPoint();
                    break;
                case TrafficType.Right:
                    bool ifOnlyRight = !_thisTrafficType.HasFlag(TrafficType.Forward) &&
                                      !_thisTrafficType.HasFlag(TrafficType.Left);
                    if (ifOnlyRight)
                    {
                        targetPoint = GetRightEmptyPoint();
                        break;
                    }

                    bool isRight = Random.Range(0f, 1f) < 0.5f;

                    targetPoint = isRight ? GetLeftEmptyPoint() : GetRightEmptyPoint();
                    break;
                case TrafficType.Forward:
                default:
                    // * 직진이 가능한 이상, 어느 차선이던 상관 없다.
                    bool getLeftOrRight = Random.Range(0f, 1f) < 0.5f;
                    targetPoint = getLeftOrRight ? GetLeftEmptyPoint() : GetRightEmptyPoint();
                    break;
            }

            // 최종적으로 결정된 해당 위치까지 차량을 출발시킨다.
            StartCoroutine(insideCar.MoveToTarget(targetPoint ? targetPoint : insideCar.transform));

            Transform GetLeftEmptyPoint()
            {
                for (int i = 0; i < LeftCarPoints.Count; i++)
                {
                    if (_leftCarPlaced[i])
                    {
                        continue;
                    }

                    _leftCarPlaced[i] = true; // 이 곳을 채워준다.
                    return LeftCarPoints[i];
                }

                return null;
            }

            Transform GetRightEmptyPoint()
            {
                for (int i = 0; i < RightCarPoints.Count; i++)
                {
                    if (_rightCarPlaced[i])
                    {
                        continue;
                    }

                    _rightCarPlaced[i] = true; // 이 곳을 채워준다.
                    return RightCarPoints[i];
                }

                return null;
            }
        }


        private void OnCarExitEvent(CarHandler insideCar)
        {
            // 차가 어디로 가야할 지 알려주고, 출발시킨다.
            switch (insideCar.TargetType)
            {
                case TrafficType.Left:
                    // 왼쪽 경유지를 들른 후, 다음으로 넘어갈 수 있도록 한다.
                    StartCoroutine(insideCar.MoveViaWaypoint(leftCarTargets[0], _leftWayPoint));
                    break;
                case TrafficType.Right:
                    StartCoroutine(insideCar.MoveViaWaypoint(rightCarTargets[0], _rightWayPoint));
                    break;
                case TrafficType.Forward:
                default:
                    // 왼쪽에 있는지, 오른쪽에 있는지 어케 알까.
                    bool isLeft = _carDetector.GetCarOnPosition(LeftCarPoints[0]);

                    if (isLeft)
                    {
                        StartCoroutine(insideCar.MoveToTarget(leftCarTargets[1]));
                        return;
                    }

                    bool isRight = _carDetector.GetCarOnPosition(RightCarPoints[0]);
                    if (isRight)
                    {
                        StartCoroutine(insideCar.MoveToTarget(rightCarTargets[1]));
                    }

                    break;
            }
        }

        private void ArrangeAllCars()
        {
            // 차가 처음 들어올 때 경우와

            // 차가 나갔을 때 경우가 있지?

            // 들어왔을 때는 지만 그냥 이동하면 돼

            // 하지만 나갔을 때는, 전체가 이동을 해줘야해.


            // 차량 리스트 내부에서, 각 차가 원하는 방향에 맞게 차선을 분배해주면 된다.
            switch (_thisTrafficType)
            {
                case TrafficType.Left:
                    // 차선 두 개다 왼쪽으로 도는 차선.

                    break;
                case TrafficType.Right:
                    // 차선 두 개 전부 오른쪽으로 도는 차선
                    break;
                case TrafficType.Left | TrafficType.Right:
                    // 차선 왼쪽이나 오른쪽 다 상관 없는 곳
                    break;
                case TrafficType.Forward | TrafficType.Left | TrafficType.Right:
                    // 직진까지 가능해진 경우
                    break;
                case TrafficType.Forward:
                default:
                    // 이 경우는 잘 없다.
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