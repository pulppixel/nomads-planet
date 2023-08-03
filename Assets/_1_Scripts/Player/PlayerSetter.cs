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

        private void Start()
        {
            setCharacterType = (CharacterType)Random.Range(0, 8);
        }

        public override void OnNetworkSpawn()
        {
            var character = Instantiate(playerPrefabs[(int)setCharacterType], this.transform);
            character.transform.SetSiblingIndex(2);
            character.gameObject.name = "Player_Model";
            _animator.Rebind();
        }
    }
}