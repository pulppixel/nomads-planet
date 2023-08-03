using System;
using System.Collections.Generic;
using NomadsPlanet.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NomadsPlanet
{
    public class PlayerSetter : MonoBehaviour
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
            var character = Instantiate(playerPrefabs[(int)setCharacterType], this.transform);
            character.transform.SetSiblingIndex(2);
            character.gameObject.name = "Player_Model";
            _animator.Rebind();
        }
    }
}