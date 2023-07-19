using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using NomadsPlanet.Utils;
using Random = UnityEngine.Random;

namespace NomadsPlanet
{
    // todo: changeValue (https://blog.naver.com/PostView.naver?blogId=dooya-log&logNo=221757539272&categoryNo=9&parentCategoryNo=0)
    // todo: 위 기능 써서 타겟 방향만 업데이트해주면 될 것 같어. (아예 변수로 뺴고, 메소드는 반복해)
    
    public class CarHandler : MonoBehaviour
    {
        // Null 체크 대용으로 쓰기 위함
        public static CarHandler NullCar => null;

        public TrafficType TargetType { get; private set; }
        public LaneType CurLaneType { get; private set; }

        public bool IsMoving { get; private set; }
        
        private Coroutine _moveCoroutine;

        public void SetTrafficTarget(TrafficType trafficType) => TargetType = trafficType;

        private Transform _carTransform;

        private void Awake()
        {
            _carTransform = GetComponent<Transform>();
            IsMoving = false;
        }

        public void MoveToTarget(Transform targetPos, LaneType laneType, float speed = 5f)
        {
            _StopAll();
            _moveCoroutine = StartCoroutine(_MoveToTarget(targetPos, laneType, speed));
        }

        public void MoveViaWaypoint(Transform targetPos, Transform wayPoint, float speed = 2.5f)
        {
            _StopAll();
            _moveCoroutine = StartCoroutine(_MoveViaWaypoint(targetPos, wayPoint, speed));
        }

        private IEnumerator _MoveToTarget(Transform targetPos, LaneType laneType, float speed = 4f)
        {
            IsMoving = true;
            CurLaneType = laneType;

            // Movement
            var position = targetPos.position;
            var target = new Vector3(position.x, -1f, position.z);

            Sequence s = DOTween.Sequence();

            s.Append(_carTransform.DOMove(target, speed));
            s.Join(_carTransform.DOLookAt(target, speed * .5f));
            s.SetSpeedBased(true).SetEase(Ease.InOutSine);

            yield return s.WaitForCompletion();
            IsMoving = false;
        }

        private IEnumerator _MoveViaWaypoint(Transform targetPos, Transform wayPoint, float speed = 2f)
        {
            IsMoving = true;
            var position = wayPoint.position;
            var target = new Vector3(position.x, -1f, position.z);

            Sequence s = DOTween.Sequence();

            s.Append(_carTransform.DOMoveX(target.x, speed).SetEase(Random.value < .5 ? Ease.OutQuad : Ease.InQuad));
            s.Join(_carTransform.DOMoveZ(target.z, speed).SetEase(Random.value < .5 ? Ease.OutQuad : Ease.InQuad));
            s.Join(_carTransform.DOLookAt(target, speed * .5f)).SetEase(Ease.InOutSine);

            s.SetSpeedBased(true);

            yield return s.WaitForCompletion();

            // Movement
            position = targetPos.position;
            target = new Vector3(position.x, -1f, position.z);

            s = DOTween.Sequence();

            s.Append(_carTransform.DOMoveX(target.x, speed).SetEase(Random.value < .5 ? Ease.OutQuad : Ease.InQuad));
            s.Join(_carTransform.DOMoveZ(target.z, speed).SetEase(Random.value < .5 ? Ease.OutQuad : Ease.InQuad));
            s.Join(_carTransform.DOLookAt(target, speed * .75f)).SetEase(Ease.InOutSine);

            s.SetSpeedBased(true);

            yield return s.WaitForCompletion();
            IsMoving = false;
        }

        private void _StopAll()
        {   
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
            }
            
            if (DOTween.IsTweening(this))
            {
                DOTween.Kill(this);
            }
        }
    }
}