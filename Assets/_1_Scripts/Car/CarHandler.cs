using System.Collections;
using UnityEngine;
using DG.Tweening;
using NomadsPlanet.Utils;

// todo: 차선 변경 ㅈ같음. 바로 옆으로 가고, 앞으로 나아가도록 하자
// todo: 회전만 어떻게 잘 잡으면 될 것 같음. y축으로만 이동 시켜 DORotateY 같은 걸로
// todo: Move 저거 굳이 SetParents 안해도 정상 동작하네
// todo: 이동 급발진해. 저거 고쳐

namespace NomadsPlanet
{
    public class CarHandler : MonoBehaviour
    {
        // Null 체크 대용으로 쓰기 위함
        public static CarHandler NullCar { get; private set; } = new();

        public TrafficType TargetType { get; private set; }
        public LaneType CurLaneType { get; private set; }

        public void SetTrafficTarget(TrafficType trafficType) => TargetType = trafficType;

        private Transform _carTransform;

        private void Awake()
        {
            _carTransform = GetComponent<Transform>();
        }


        public IEnumerator MoveToTarget(Transform targetPos, LaneType laneType, float speed = 5)
        {
            CurLaneType = laneType;

            if (DOTween.IsTweening(this))
            {
                DOTween.Kill(this);
            }

            // 타겟 오브젝트의 자식으로 들어가서 이동한다.
            // _carTransform.SetParent(targetPos);

            // Movement
            var position = targetPos.position;
            var carTween = _carTransform.DOMove(new Vector3(position.x, _carTransform.position.y, position.z), speed)
                .SetSpeedBased(true);

            _carTransform.DOLocalRotate(targetPos.rotation.eulerAngles, 1f);

            yield return carTween.WaitForCompletion();
        }

        public IEnumerator MoveViaWaypoint(Transform targetPos, Transform wayPoint, float speed = 5)
        {
            if (DOTween.IsTweening(this))
            {
                DOTween.Kill(this);
            }

            var getQuaternion = _GetTurnQuaternion(targetPos);

            // 타겟 좌표로
            // _carTransform.SetParent(wayPoint);

            var position = wayPoint.position;
            var carTween = _carTransform.DOMove(new Vector3(position.x, _carTransform.position.y, position.z), speed)
                .SetSpeedBased(true);

            _carTransform.DORotateQuaternion(getQuaternion, 1f);

            yield return carTween.WaitForCompletion();
            yield return MoveToTarget(targetPos, CurLaneType, speed);
        }

        private Quaternion _GetTurnQuaternion(Transform targetPos)
        {
            Vector3 direction = (targetPos.position - _carTransform.position).normalized;
            return Quaternion.LookRotation(new Vector3(0, direction.y, 0));
        }
    }
}