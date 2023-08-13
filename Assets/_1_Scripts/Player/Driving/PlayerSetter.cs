using System.Collections.Generic;
using NomadsPlanet.Utils;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace NomadsPlanet
{
    public class PlayerSetter : NetworkBehaviour
    {
        [SerializeField] private CharacterType setCharacterType;
        [SerializeField] private MeshCollider collider;

        public NetworkVariable<FixedString32Bytes> playerName = new();

        private Vector3 _size;
        private readonly Collider[] _sizeBuffer = new Collider[1];

        private Animator _animator;
        private readonly List<GameObject> _playerPrefabs = new();
        private readonly NetworkVariable<CharacterType> _playerType = new();

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            var parent = transform.GetChildFromName<Transform>("CharacterPrefabs");
            _size = collider.sharedMesh.bounds.size * 1.5f;
            for (int i = 0; i < parent.childCount; i++)
            {
                _playerPrefabs.Add(parent.GetChild(i).gameObject);
            }

            collider.enabled = false;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                var userData = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
                playerName.Value = userData.userName;
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
            collider.enabled = true;

            int idx = (int)newValue;
            _playerPrefabs[idx].transform.SetParent(transform);
            _playerPrefabs[idx].transform.SetSiblingIndex(2);
            _playerPrefabs[idx].gameObject.name = "Player_Model";
            _playerPrefabs[idx].gameObject.SetActive(true);

            _animator.Rebind();
        }

        private Vector3 GetSpawnPoint()
        {
            List<Vector3> possibleSpawns = new List<Vector3>();

            for (int i = 0; i < 100; i++) // 예: 최대 100번의 시도
            {
                float x = Random.Range(-50, 50);
                float z = Random.Range(-50, 50);

                Vector3 spawnPoint = new Vector3(x, 1f, z);

                if (IsSpawnPointFree(spawnPoint))
                {
                    possibleSpawns.Add(spawnPoint);
                }
            }

            return possibleSpawns.Count > 0
                ? possibleSpawns[Random.Range(0, possibleSpawns.Count)]
                : new Vector3(Random.value < .5f ? -25f : 25f, 1f, Random.value < .5f ? -25f : 25f);
        }

        private bool IsSpawnPointFree(Vector3 spawnPoint)
        {
            int numColliders = Physics.OverlapBoxNonAlloc(spawnPoint, _size, _sizeBuffer);

            return numColliders == 0;
        }
    }
}