using System;
using UnityEngine;
using Unity.Netcode;

namespace NomadsPlanet
{
    public class PlayerWeapon : NetworkBehaviour
    {
        private const float ForceMultiplier = 5000f;
        private const int Damage = 5;
        private float _lastAttackTime = -3.0f;
        private const float AttackCooldown = 3.0f;

        public event Action<PlayerWeapon> OnAttack;
        public event Action<PlayerWeapon> OnDamaged;
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
            if (!IsOwner)
            {
                return;
            }

            if (Time.time - _lastAttackTime < AttackCooldown)
            {
                return;
            }

            if (other.attachedRigidbody == null || !other.gameObject.CompareTag("Player"))
            {
                return;
            }

            _lastAttackTime = Time.time;
            SoundManager.Instance.PlayHitSoundSfx();

            // 상대방 몸통 박았을 경우
            if (other.TryGetComponent<CarParentGetter>(out var enemy))
            {
                var netObj = enemy.PlayerScore.GetComponent<NetworkObject>();

                if (_ownerClientId == netObj.OwnerClientId)
                {
                    return;
                }

                // _playerScore.GetScore(Damage);
                enemy.PlayerScore.LostScore(Damage);
                enemy.PlayerScore.playerWeapon.OnDamaged?.Invoke(enemy.PlayerScore.playerWeapon);
                OnAttack?.Invoke(this);
            }

            // 상대방 정면 박았을 경우
            if (other.TryGetComponent<PlayerWeapon>(out var weapon))
            {
                OnAttack?.Invoke(this);
                // _playerScore.GetScore(Damage);
            }

            Vector3 forceDirection = other.transform.position - transform.position;
            forceDirection.Normalize();

            _rigidbody.AddForce(-forceDirection * ForceMultiplier, ForceMode.Impulse);
        }
    }
}