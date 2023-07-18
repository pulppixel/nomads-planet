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


        public IEnumerator MoveToTarget(Transform targetPos, LaneType laneType, float speed = 5f)
        {
            CurLaneType = laneType;

            if (DOTween.IsTweening(this))
            {
                DOTween.Kill(this);
            }

            // Movement
            var position = targetPos.position;
            var target = new Vector3(position.x, _carTransform.position.y, position.z);
            var carTween = _carTransform.DOMove(target, speed)
                .SetSpeedBased(true);

            // 여기서는 회전 넣지 말까봐
            yield return carTween.WaitForCompletion();
        }

        public IEnumerator MoveViaWaypoint(Transform targetPos, Transform wayPoint, float speed = 5f)
        {
            if (DOTween.IsTweening(this))
            {
                DOTween.Kill(this);
            }

            var position = wayPoint.position;
            var target = new Vector3(position.x, _carTransform.position.y, position.z);
            var carTween = _carTransform.DOMove(target, speed)
                .SetSpeedBased(true);

            _carTransform.DORotate(_GetTurnQuaternion(45f), speed * 1.85f)
                .SetSpeedBased(true);

            yield return carTween.WaitForCompletion();

            // Movement
            position = targetPos.position;
            target = new Vector3(position.x, _carTransform.position.y, position.z);
            carTween = _carTransform.DOMove(target, speed)
                .SetSpeedBased(true);

            _carTransform.DORotate(_GetTurnQuaternion(45f), speed * 1.85f)
                .SetSpeedBased(true);

            // 여기서는 회전 넣지 말까봐
            yield return carTween.WaitForCompletion();
        }

        private Vector3 _GetTurnQuaternion(float amount = 90f)
        {
            var curRotate = _carTransform.rotation.eulerAngles;
            float targetRotateY;

            switch (TargetType)
            {
                case TrafficType.Left:
                    targetRotateY = curRotate.y - amount;
                    break;
                case TrafficType.Right:
                    targetRotateY = curRotate.y + amount;
                    break;
                case TrafficType.Forward:
                default:
                    targetRotateY = curRotate.y;
                    break;
            }

            return new Vector3(curRotate.x, targetRotateY, curRotate.z);
        }
    }
}