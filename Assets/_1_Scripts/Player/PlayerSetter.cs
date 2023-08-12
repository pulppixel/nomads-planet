using System;
using System.Collections.Generic;
using NomadsPlanet.Utils;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NomadsPlanet
{
    public class PlayerSetter : NetworkBehaviour
    {
        public List<GameObject> playerPrefabs;
        public CharacterType setCharacterType;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                SetCharacterServerRpc();
            }
        }

        [ServerRpc]
        private void SetCharacterServerRpc()
        {
            setCharacterType = (CharacterType)Random.Range(0, 8);
            PropagateCharacterToClientsClientRpc((int)setCharacterType);
        }

        [ClientRpc]
        private void PropagateCharacterToClientsClientRpc(int characterIndex)
        {
            if (playerPrefabs == null || characterIndex < 0 || characterIndex >= playerPrefabs.Count)
                return;

            var character = Instantiate(playerPrefabs[characterIndex], this.transform);
            character.transform.SetSiblingIndex(2);
            character.gameObject.name = "Player_Model";
            _animator.Rebind();
        }
    }
}