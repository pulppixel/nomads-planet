using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using NomadsPlanet.Utils;

namespace NomadsPlanet
{
    public class CarHandler : MonoBehaviour
    {
        // Null 체크 대용으로 쓰기 위함
        public static CarHandler NullCar { get; } = null;

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
            var target = new Vector3(position.x, _carTransform.position.y, position.z);

            Sequence s = DOTween.Sequence();

            s.Append(_carTransform.DOMove(position, speed));
            s.Join(_carTransform.DOLookAt(target, speed * .5f));
            s.SetSpeedBased(true).SetEase(Ease.Linear);

            yield return s.WaitForCompletion();
            IsMoving = false;
        }

        private IEnumerator _MoveViaWaypoint(Transform targetPos, Transform wayPoint, float speed = 2f)
        {
            IsMoving = true;
            var position = wayPoint.position;
            var target = new Vector3(position.x, _carTransform.position.y, position.z);

            Sequence s = DOTween.Sequence();

            s.Append(_carTransform.DOMoveX(position.x, speed).SetEase(Ease.OutSine));
            s.Join(_carTransform.DOMoveZ(position.z, speed).SetEase(Ease.InSine));
            s.Join(_carTransform.DOLookAt(target, speed * .5f));

            s.SetSpeedBased(true);

            yield return s.WaitForCompletion();

            // Movement
            position = targetPos.position;
            target = new Vector3(position.x, _carTransform.position.y, position.z);

            s = DOTween.Sequence();

            s.Append(_carTransform.DOMoveX(position.x, speed).SetEase(Ease.InSine));
            s.Join(_carTransform.DOMoveZ(position.z, speed).SetEase(Ease.OutSine));
            s.Join(_carTransform.DOLookAt(target, speed * .75f));

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