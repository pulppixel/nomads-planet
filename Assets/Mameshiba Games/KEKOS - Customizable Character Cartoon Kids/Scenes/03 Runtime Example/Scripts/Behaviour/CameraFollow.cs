using UnityEngine;

namespace MameshibaGames.Kekos.RuntimeExampleScene.Behaviour
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField]
        private Transform objectToFollow;

        private Transform _transform;
        private Vector3 _previousPosition;

        private void Awake()
        {
            _transform = transform;
            _previousPosition = objectToFollow.position;
        }

        private void LateUpdate()
        {
            Vector3 objectToFollowPosition = objectToFollow.position;
            Vector3 offset = objectToFollowPosition - _previousPosition;
            _previousPosition = objectToFollowPosition;

            offset.y = 0;

            _transform.position += offset;
        }
    }
}
