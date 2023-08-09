using UnityEngine;
using Unity.Netcode;

namespace NomadsPlanet
{
    public class PlayerWeapon : MonoBehaviour
    {
        private const float ForceMultiplier = 5000f;
        private const int Damage = 5;

        private PlayerScore _playerScore;

        private ulong _ownerClientId;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = transform.parent.GetComponent<Rigidbody>();
            _playerScore = _rigidbody.GetComponent<PlayerScore>();
        }

        private void Start()
        {
            this._ownerClientId = _playerScore.OwnerClientId;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody == null || !other.gameObject.CompareTag("Player"))
            {
                return;
            }

            // 상대방 몸통 박았을 경우
            if (other.TryGetComponent<CarParentGetter>(out var enemy))
            {
                var netObj = enemy.PlayerScore.GetComponent<NetworkObject>();

                if (_ownerClientId == netObj.OwnerClientId)
                {
                    return;
                }

                enemy.PlayerScore.LostScore(Damage);
                _playerScore.GetScore(Damage);
            }

            // 상대방 정면 박았을 경우
            if (other.TryGetComponent<PlayerWeapon>(out var weapon))
            {
                _playerScore.GetScore(Damage);
            }

            Vector3 forceDirection = other.transform.position - transform.position;
            forceDirection.Normalize();

            _rigidbody.AddForce(-forceDirection * ForceMultiplier, ForceMode.Impulse);
        }
    }
}