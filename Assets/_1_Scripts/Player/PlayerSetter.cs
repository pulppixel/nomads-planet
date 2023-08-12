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
        public CharacterType setCharacterType;
        private Animator _animator;
        private List<GameObject> _playerPrefabs = new();

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
            Debug.Log("OnNetworkSpawn called on client.");
            setCharacterType = (CharacterType)Random.Range(0, 8);
            int idx = (int)setCharacterType;
            _playerPrefabs[idx].transform.SetParent(transform);
            _playerPrefabs[idx].transform.SetSiblingIndex(2);
            _playerPrefabs[idx].gameObject.name = "Player_Model";
            _playerPrefabs[idx].gameObject.SetActive(true);
            _animator.Rebind();
        }
    }
}