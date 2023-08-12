using System.Collections.Generic;
using NomadsPlanet.Utils;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NomadsPlanet
{
    public class PlayerSetter : NetworkBehaviour
    {
        public CharacterType setCharacterType;

        private Animator _animator;
        private readonly List<GameObject> _playerPrefabs = new();
        private readonly NetworkVariable<CharacterType> _playerType = new();

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            var parent = transform.GetChildFromName<Transform>("CharacterPrefabs");
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
            int idx = (int)newValue;
            _playerPrefabs[idx].transform.SetParent(transform);
            _playerPrefabs[idx].transform.SetSiblingIndex(2);
            _playerPrefabs[idx].gameObject.name = "Player_Model";
            _playerPrefabs[idx].gameObject.SetActive(true);
            _animator.Rebind();
        }
    }
}