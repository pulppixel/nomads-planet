using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace NomadsPlanet
{
    public class CarHandler : MonoBehaviour
    {
        // Null 체크 대용으로 쓰기 위함
        public static CarHandler NullCar => null; // ㅋㅋ

        private const float Speed = .15f;

        [ShowInInspector] public bool IsMoving { get; private set; }

        private Transform _transform;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        // 차선 내부에서 이동함
        public void MoveToTarget(Vector3 targetPos, bool isLinear, float delay = 0f)
        {
            IsMoving = true;
            DOTween.Kill(this);

            float duration = Vector3.Distance(_transform.position, targetPos) * Speed * Random.Range(1f, 1.2f);
            DOTween.Sequence()
                .Append(_transform.DOMove(new Vector3(targetPos.x, 0, targetPos.z), duration))
                .Join(_transform.DOLookAt(new Vector3(targetPos.x, 0, targetPos.z), duration * .8f))
                .SetEase(isLinear ? Ease.Linear : Ease.OutQuad)
                .SetDelay(delay)
                .OnComplete(() =>
                {
                    IsMoving = false;
                    DOTween.Kill(this);
                });
        }

        public void MoveViaWaypoint(Vector3 targetPos, Vector3[] wayPoint, bool isLinear, float delay = 0f)
        {
            IsMoving = true;
            DOTween.Kill(this);

            float dis1 = Vector3.Distance(_transform.position, wayPoint[0]);
            float dis2 = Vector3.Distance(wayPoint[0], wayPoint[1]);
            float dis3 = Vector3.Distance(wayPoint[1], targetPos);

            float duration = (dis1 + dis2 + dis3) * Speed * Random.Range(1f, 1.2f);
            var path = new Vector3[]
            {
                new(wayPoint[0].x, 0, wayPoint[0].z),
                new(wayPoint[1].x, 0, wayPoint[1].z),
                new(targetPos.x, 0, targetPos.z),
            };

            _transform.DOPath(path, duration, PathType.CatmullRom)
                .SetEase(isLinear ? Ease.Linear : Ease.OutQuad)
                .SetLookAt(.001f)
                .SetDelay(delay)
                .OnUpdate(() =>
                {
                    IsMoving = false;
                    DOTween.Kill(this);
                });

            // _carTransform.DOLookAt(new Vector3(targetPos.x, -1, targetPos.z), duration * .8f);
        }
    }
}