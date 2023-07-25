using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace NomadsPlanet
{
    public class CarHandler : MonoBehaviour
    {
        // Null 체크 대용으로 쓰기 위함
        public static CarHandler NullCar => null; // ㅋㅋ

        private const float Speed = .14f;

        private Transform _carTransform;

        private void Awake()
        {
            _carTransform = GetComponent<Transform>();
        }

        // 차선 내부에서 이동함
        public void MoveToTarget(Vector3 targetPos, bool isLinear, float delay = 0f)
        {
            if (DOTween.IsTweening(this))
            {
                DOTween.Kill(this);
            }

            float duration = Vector3.Distance(_carTransform.position, targetPos) * Speed;
            DOTween.Sequence()
                .Append(_carTransform.DOMove(new Vector3(targetPos.x, -1, targetPos.z), duration))
                .Join(_carTransform.DOLookAt(new Vector3(targetPos.x, -1, targetPos.z), duration * .8f))
                .SetEase(isLinear ? Ease.Linear : Ease.OutQuad)
                .SetDelay(delay);
        }

        public void MoveViaWaypoint(Vector3 targetPos, Vector3[] wayPoint, bool isLinear, float delay = 0f)
        {
            if (DOTween.IsTweening(this))
            {
                DOTween.Kill(this);
            }

            float dis1 = Vector3.Distance(_carTransform.position, wayPoint[0]);
            float dis2 = Vector3.Distance(wayPoint[0], wayPoint[1]);
            float dis3 = Vector3.Distance(wayPoint[1], targetPos);

            float duration = (dis1 + dis2 + dis3) * Speed;
            var path = new Vector3[]
            {
                new(wayPoint[0].x, -1, wayPoint[0].z),
                new(wayPoint[1].x, -1, wayPoint[1].z),
                new(targetPos.x, -1, targetPos.z),
            };

            _carTransform.DOPath(path, duration, PathType.CatmullRom)
                .SetEase(isLinear ? Ease.Linear : Ease.OutQuad)
                .SetLookAt(.001f)
                .SetDelay(delay);

            // _carTransform.DOLookAt(new Vector3(targetPos.x, -1, targetPos.z), duration * .8f);
        }
    }
}