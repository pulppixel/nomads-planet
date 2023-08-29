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

        public static event Action<DrivingPlayer> OnPlayerSpawned;
        public static event Action<DrivingPlayer> OnPlayerDespawned;

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
                int charIdx = ES3.Load(PrefsKey.AvatarTypeKey, Random.Range(0, 8));
                int carIdx = ES3.Load(PrefsKey.CarTypeKey, Random.Range(0, 8));

                CloseAllPrefabs();
                SetCharacterAndCarTypeServerRpc(charIdx, carIdx);
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
        
        [ServerRpc(RequireOwnership = false)]
        private void SetCharacterAndCarTypeServerRpc(int charIdx, int carIdx)
        {
            characterType.Value = charIdx;
            carType.Value = carIdx;
            UpdateCharacter(charIdx, carIdx);
        }

        private void UpdateCharacter(int charIdx, int catIdx)
        {
            playerPrefabs[charIdx].gameObject.SetActive(true);
            playerPrefabs[charIdx].transform.SetParent(transform);
            playerPrefabs[charIdx].transform.SetSiblingIndex(2);
            playerPrefabs[charIdx].gameObject.name = "Player_Model";

            carPrefabs[catIdx].gameObject.SetActive(true);
            carPrefabs[catIdx].transform.SetParent(transform);
            carPrefabs[catIdx].transform.SetSiblingIndex(3);
            carPrefabs[catIdx].gameObject.name = "Player_Car";

            GetComponent<Animator>().Rebind();
            CarController.Init(carPrefabs[catIdx].transform);
        }

        private void CloseAllPrefabs()
        {
            foreach (var prefab in playerPrefabs)
            {
                prefab.SetActive(false);
            }

            foreach (var prefab in carPrefabs)
            {
                prefab.SetActive(false);
            }
        }
    }
}