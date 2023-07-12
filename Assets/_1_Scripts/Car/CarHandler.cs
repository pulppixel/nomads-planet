using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace NomadsPlanet
{
    public class CarHandler : MonoBehaviour
    {
        private Transform _carTransform;
        public Transform target;
        public Transform center;
        public bool moveToCenter;

        private void Awake()
        {
            _carTransform = GetComponent<Transform>();
        }

        public IEnumerator MoveToTarget(Transform targetPos, float speed = 10)
        {
            if (DOTween.IsTweening(this))
            {
                DOTween.Kill(this);
            }

            // 타겟 좌표로
            _carTransform.SetParent(targetPos);

            // Movement
            var carTween = _carTransform.DOLocalMove(Vector3.zero, speed)
                .SetSpeedBased(true);

            _carTransform.DOLocalRotate(Vector3.zero, 1f);

            yield return carTween.WaitForCompletion();
        }

        public IEnumerator MoveToWaypoint(Transform targetPos, Transform wayPoint, float speed = 10)
        {
            if (DOTween.IsTweening(this))
            {
                DOTween.Kill(this);
            }

            var getQuaternion = _GetTurnQuaternion(targetPos);
            _carTransform.DORotateQuaternion(getQuaternion, 1f);

            // 타겟 좌표로
            _carTransform.SetParent(wayPoint);

            var carTween = _carTransform.DOLocalMove(Vector3.zero, speed)
                .SetSpeedBased(true);

            yield return carTween.WaitForCompletion();
        }

        private Quaternion _GetTurnQuaternion(Transform targetPos)
        {
            Vector3 direction = (targetPos.position - _carTransform.position).normalized;
            return Quaternion.LookRotation(direction);
        }
    }
}