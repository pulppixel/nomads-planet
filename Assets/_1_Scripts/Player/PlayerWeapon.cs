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

            if (other.TryGetComponent<CarColliderGetter>(out var carDetector))
            {
                var netObj = carDetector.CarHandler.GetComponent<NetworkObject>();

                if (_ownerClientId == netObj.OwnerClientId)
                {
                    return;
                }

                var otherScore = carDetector.CarHandler.GetComponent<PlayerScore>();
                otherScore.LostScore(Damage);
            }

            Vector3 forceDirection = other.transform.position - transform.position;
            forceDirection.Normalize();

            _rigidbody.AddForce(-forceDirection * ForceMultiplier, ForceMode.Impulse);
            _playerScore.GetScore(Damage * 3);
        }
    }
}