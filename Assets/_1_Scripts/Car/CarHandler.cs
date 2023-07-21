using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace NomadsPlanet
{
    public class CarHandler : MonoBehaviour
    {
        // Null 체크 대용으로 쓰기 위함
        public static CarHandler NullCar => null;

        // 차량 스피드
        private const float Speed = .1f;
        private Ease _ease = Ease.InOutQuad;

        // 이동에 필요한 Tween들
        private Tweener _moveTween;
        private Tweener _lookTween;

        // 타겟 값
        private Vector3 _oldPosition;
        private Vector3 _targetPosition;

        private Transform _carTransform;

        private void Awake()
        {
            _carTransform = GetComponent<Transform>();
        }

        private void OnEnable()
        {
            var position = _carTransform.position;
            _targetPosition = new Vector3(position.x, -1f, position.z);
            _oldPosition = _targetPosition;
            _moveTween = _carTransform.DOMove(_targetPosition, 1f).SetAutoKill(false);
            _lookTween = _carTransform.DOLookAt(_targetPosition, 1f).SetAutoKill(false);
        }

        private void Update()
        {
            if (Vector3.Distance(_oldPosition, _targetPosition) > 1f)
            {
                var position = _targetPosition;
                _targetPosition = new Vector3(position.x, -1f, position.z);
                _oldPosition = _targetPosition;

                float duration = Vector3.Distance(_carTransform.position, _targetPosition) * Speed;

                _moveTween.ChangeEndValue(_targetPosition, duration, true)
                    .SetEase(_ease)
                    .SetAutoKill(false)
                    .Restart();

                _lookTween.ChangeEndValue(_targetPosition, duration * .75f, true)
                    .SetEase(Ease.Linear)
                    .SetAutoKill(false)
                    .Restart();
            }
        }

        // 차선 내부에서 이동함
        public void MoveToTarget(Transform targetPos)
        {
            _ease = Ease.OutSine;
            _targetPosition = targetPos.position;
        }

        public void MoveViaWaypoint(Transform targetPos, Transform wayPoint)
        {
            _ease = Ease.Linear;
            _targetPosition = wayPoint.position;
            StartCoroutine(Move());

            IEnumerator Move()
            {
                yield return new WaitUntil(() => Vector3.Distance(_carTransform.position, wayPoint.position) < 1f);
                _targetPosition = targetPos.position;
            }
        }
    }
}