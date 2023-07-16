using System.Collections;
using UnityEngine;
using DG.Tweening;
using NomadsPlanet.Utils;

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


        public IEnumerator MoveToTarget(Transform targetPos, LaneType laneType, float speed = 10)
        {
            CurLaneType = laneType;

            if (DOTween.IsTweening(this))
            {
                DOTween.Kill(this);
            }

            // 타겟 오브젝트의 자식으로 들어가서 이동한다.
            _carTransform.SetParent(targetPos);

            // Movement
            var carTween = _carTransform.DOLocalMove(Vector3.zero, speed)
                .SetSpeedBased(true);

            _carTransform.DOLocalRotate(Vector3.zero, 1f);

            yield return carTween.WaitForCompletion();
        }

        public IEnumerator MoveViaWaypoint(Transform targetPos, Transform wayPoint, float speed = 10)
        {
            if (DOTween.IsTweening(this))
            {
                DOTween.Kill(this);
            }

            var getQuaternion = _GetTurnQuaternion(targetPos);

            // 타겟 좌표로
            _carTransform.SetParent(wayPoint);

            var carTween = _carTransform.DOLocalMove(Vector3.zero, speed)
                .SetSpeedBased(true);

            _carTransform.DORotateQuaternion(getQuaternion, 1f);

            yield return carTween.WaitForCompletion();
            yield return MoveToTarget(targetPos, CurLaneType, speed);
        }

        private Quaternion _GetTurnQuaternion(Transform targetPos)
        {
            Vector3 direction = (targetPos.position - _carTransform.position).normalized;
            return Quaternion.LookRotation(direction);
        }
    }
}