using System.Collections.Generic;
using NomadsPlanet.Utils;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NomadsPlanet
{
    public class PlayerSetter : NetworkBehaviour
    {
        [SerializeField] private CharacterType setCharacterType;
        [SerializeField] private MeshCollider collider;

        private Vector3 _size;
        private readonly Collider[] _sizeBuffer = new Collider[1];

        private Animator _animator;
        private readonly List<GameObject> _playerPrefabs = new();
        private readonly NetworkVariable<CharacterType> _playerType = new();

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            var parent = transform.GetChildFromName<Transform>("CharacterPrefabs");
            _size = collider.sharedMesh.bounds.size;
            for (int i = 0; i < parent.childCount; i++)
            {
                _playerPrefabs.Add(parent.GetChild(i).gameObject);
            }
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                SetPlayerTypeOnServerRpc();
            }
            else
            {
                UpdateCharacter(_playerType.Value);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerTypeOnServerRpc()
        {
            setCharacterType = (CharacterType)Random.Range(0, 8);
            _playerType.Value = setCharacterType;
            UpdateCharacter(setCharacterType);
        }

        private void UpdateCharacter(CharacterType newValue)
        {
            transform.position = GetSpawnPoint();

            int idx = (int)newValue;
            _playerPrefabs[idx].transform.SetParent(transform);
            _playerPrefabs[idx].transform.SetSiblingIndex(2);
            _playerPrefabs[idx].gameObject.name = "Player_Model";
            _playerPrefabs[idx].gameObject.SetActive(true);
            _animator.Rebind();
        }

        private Vector3 GetSpawnPoint()
        {
            float x = Random.Range(-25, 25);
            float z = Random.Range(-25, 25);

            Vector3 spawnPoint = new Vector3(x, 0, z);
            return IsSpawnPointFree(spawnPoint) ? spawnPoint : GetSpawnPoint();
        }

        private bool IsSpawnPointFree(Vector3 spawnPoint)
        {
            int numColliders = Physics.OverlapBoxNonAlloc(spawnPoint, _size, _sizeBuffer);

            for (int i = 0; i < numColliders; i++)
            {
                if (_sizeBuffer[i].CompareTag("Coin"))
                {
                    continue;
                }

                return true;
            }

            return false;
        }
    }
}