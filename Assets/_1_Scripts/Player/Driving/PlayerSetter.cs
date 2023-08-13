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
        public NetworkVariable<FixedString32Bytes> playerName = new();
        public new Collider collider;

        private Animator _animator;
        private readonly List<GameObject> _playerPrefabs = new();
        private readonly NetworkVariable<CharacterType> _playerType = new();

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            collider.enabled = false;
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
                var userData = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);

                playerName.Value = userData.userName;
                _playerType.Value = (CharacterType)userData.userAvatarType;
            }

            UpdateCharacter(_playerType.Value);
        }

        private void UpdateCharacter(CharacterType newValue)
        {
            int idx = (int)newValue;
            _playerPrefabs[idx].transform.SetParent(transform);
            _playerPrefabs[idx].transform.SetSiblingIndex(2);
            _playerPrefabs[idx].gameObject.name = "Player_Model";
            _playerPrefabs[idx].gameObject.SetActive(true);

            _animator.Rebind();
            collider.enabled = true;
        }
    }
}