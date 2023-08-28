using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using NomadsPlanet.Utils;
using UnityStandardAssets.Vehicles.Car;
using Random = UnityEngine.Random;

namespace NomadsPlanet
{
    public class DrivingPlayer : NetworkBehaviour
    {
        [field: SerializeField] public CoinWallet Wallet { get; private set; }
        [field: SerializeField] public CarController CarController { get; private set; }

        [SerializeField] private MeshRenderer minimapIconRenderer;
        [SerializeField] private Camera minimapCamera;
        [SerializeField] private Color ownerColor;

        public NetworkVariable<FixedString32Bytes> playerName = new();
        public NetworkVariable<FixedString32Bytes> characterType = new();
        public NetworkVariable<FixedString32Bytes> carType = new();

        private Animator _animator;
        private readonly List<GameObject> _playerPrefabs = new();
        private readonly List<GameObject> _carPrefabs = new();

        public static event Action<DrivingPlayer> OnPlayerSpawned;
        public static event Action<DrivingPlayer> OnPlayerDespawned;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            var parent = transform.GetChildFromName<Transform>("CharacterPrefabs");
            for (int i = 0; i < parent.childCount; i++)
            {
                var obj = parent.GetChild(i).gameObject;
                obj.SetActive(false);
                _playerPrefabs.Add(obj);
            }

            parent = transform.GetChildFromName<Transform>("CarPrefabs");
            for (int i = 0; i < parent.childCount; i++)
            {
                var obj = parent.GetChild(i).gameObject;
                obj.SetActive(false);
                _carPrefabs.Add(obj);
            }
        }

        public override void OnNetworkSpawn()
        {
            if (!IsLocalPlayer)
            {
                minimapCamera.gameObject.SetActive(false);
            }

            if (IsServer)
            {
#if UNITY_ANDROID || UNITY_IOS
                var userData = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
#else
                var userData = IsHost
                    ? HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId)
                    : ServerSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
#endif

                playerName.Value = userData.userName;
                characterType.Value = ES3.LoadString(PrefsKey.AvatarKey, ((CharacterType)Random.Range(0, 8)).ToString());
                carType.Value = ES3.LoadString(PrefsKey.CarKey, ((CarType)Random.Range(0, 8)).ToString());
                OnPlayerSpawned?.Invoke(this);
            }

            if (IsOwner)
            {
                minimapIconRenderer.materials[0].color = ownerColor;
            }

            UpdateCharacter();
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                OnPlayerDespawned?.Invoke(this);
            }
        }

        private void UpdateCharacter()
        {
            CharacterType character = (CharacterType)Enum.Parse(typeof(CharacterType), characterType.Value.ToString());
            CarType car = (CarType)Enum.Parse(typeof(CarType), carType.Value.ToString());

            int idx = (int)character;
            _playerPrefabs[idx].transform.SetParent(transform);
            _playerPrefabs[idx].transform.SetSiblingIndex(2);
            _playerPrefabs[idx].gameObject.name = "Player_Model";
            _playerPrefabs[idx].gameObject.SetActive(true);

            idx = (int)car;
            _carPrefabs[idx].transform.SetParent(transform);
            _carPrefabs[idx].transform.SetSiblingIndex(3);
            _carPrefabs[idx].gameObject.name = "Player_Car";
            _carPrefabs[idx].gameObject.SetActive(true);

            _animator.Rebind();
            CarController.Init(_carPrefabs[idx].transform);
        }
    }
}