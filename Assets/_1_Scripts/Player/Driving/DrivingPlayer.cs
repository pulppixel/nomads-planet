using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using NomadsPlanet.Utils;
using UnityEngine.Serialization;
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

        [SerializeField] private GameObject[] playerPrefabs;
        [SerializeField] private GameObject[] carPrefabs;

        public NetworkVariable<FixedString32Bytes> playerName = new();
        public NetworkVariable<int> characterType = new();
        public NetworkVariable<int> carType = new();

        private Animator _animator;

        public static event Action<DrivingPlayer> OnPlayerSpawned;
        public static event Action<DrivingPlayer> OnPlayerDespawned;
        
        public override void OnNetworkSpawn()
        {
            if (!IsLocalPlayer)
            {
                minimapCamera.gameObject.SetActive(false);

                foreach (var prefab in playerPrefabs)
                {
                    prefab.SetActive(false);
                }

                foreach (var prefab in carPrefabs)
                {
                    prefab.SetActive(false);
                }
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
                characterType.Value = ES3.Load(PrefsKey.AvatarTypeKey, Random.Range(0, 8));
                carType.Value = ES3.Load(PrefsKey.CarTypeKey, Random.Range(0, 8));
                UpdateCharacter();

                OnPlayerSpawned?.Invoke(this);
            }

            if (IsOwner)
            {
                minimapIconRenderer.materials[0].color = ownerColor;
            }
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
            int idx = characterType.Value;
            playerPrefabs[idx].transform.SetParent(transform);
            playerPrefabs[idx].transform.SetSiblingIndex(2);
            playerPrefabs[idx].gameObject.name = "Player_Model";
            playerPrefabs[idx].gameObject.SetActive(true);

            idx = carType.Value;
            carPrefabs[idx].transform.SetParent(transform);
            carPrefabs[idx].transform.SetSiblingIndex(3);
            carPrefabs[idx].gameObject.name = "Player_Car";
            carPrefabs[idx].gameObject.SetActive(true);

            _animator.Rebind();
            CarController.Init(carPrefabs[idx].transform);
        }
    }
}