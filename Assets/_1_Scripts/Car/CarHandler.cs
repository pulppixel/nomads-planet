using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace NomadsPlanet
{
    public class CarHandler : MonoBehaviour
    {
        // Null 체크 대용으로 쓰기 위함
        public static CarHandler NullCar => null; // ㅋㅋ

        private const float Speed = .125f;

        private Transform _carTransform;

        private void Awake()
        {
            _carTransform = GetComponent<Transform>();
        }

        // 차선 내부에서 이동함
        public void MoveToTarget(Vector3 targetPos, bool isLinear)
        {
            if (DOTween.IsTweening(this))
            {
                DOTween.Kill(this);
            }

            float duration = Vector3.Distance(_carTransform.position, targetPos) * Speed;
            DOTween.Sequence()
                .Append(_carTransform.DOMove(new Vector3(targetPos.x, -1, targetPos.z), duration))
                .Join(_carTransform.DOLookAt(new Vector3(targetPos.x, -1, targetPos.z), duration * .8f))
                .SetEase(isLinear ? Ease.Linear : Ease.OutQuad);
        }

        public void MoveViaWaypoint(Vector3 targetPos, Vector3[] wayPoint, bool isLinear)
        {
            if (DOTween.IsTweening(this))
            {
                DOTween.Kill(this);
            }

            float duration = Vector3.Distance(_carTransform.position, targetPos) * Speed * 1.5f;
            var path = new Vector3[]
            {
                new(wayPoint[0].x, -1, wayPoint[0].z),
                new(wayPoint[1].x, -1, wayPoint[1].z),
                new(targetPos.x, -1, targetPos.z),
            };

            _carTransform.DOPath(path, duration, isLinear ? PathType.CatmullRom : PathType.Linear)
                .SetEase(isLinear ? Ease.Linear : Ease.OutQuad)
                .SetLookAt(.001f);

            // _carTransform.DOLookAt(new Vector3(targetPos.x, -1, targetPos.z), duration * .8f);
        }
    }
}