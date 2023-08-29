using System;
using NomadsPlanet.Utils;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using Unity.Netcode.Components;
using UnityStandardAssets.Vehicles.Car;

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

        public NetworkVariable<FixedString32Bytes> playerName = new("");
        public NetworkVariable<int> avatarType = new();
        public NetworkVariable<int> carType = new();

        public static event Action<DrivingPlayer> OnPlayerSpawned;
        public static event Action<DrivingPlayer> OnPlayerDespawned;

        public override void OnNetworkSpawn()
        {
            if (!IsLocalPlayer)
            {
                minimapCamera.gameObject.SetActive(false);
            }

            if (IsOwner)
            {
                minimapIconRenderer.materials[0].color = ownerColor;
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
                avatarType.Value = userData.userAvatarType;
                carType.Value = userData.userCarType;
                OnPlayerSpawned?.Invoke(this);
            }
        }

        private void UpdateCharacter()
        {
            CloseAllPrefabs();
            var avatar = playerPrefabs[avatarType.Value].gameObject;
            avatar.gameObject.SetActive(true);
            avatar.transform.SetParent(transform);
            avatar.transform.SetSiblingIndex(2);
            avatar.gameObject.name = "Player_Model";

            var car = carPrefabs[carType.Value].gameObject;
            car.gameObject.SetActive(true);
            car.transform.SetParent(transform);
            car.transform.SetSiblingIndex(3);
            car.gameObject.name = "Player_Car";

            CarController.Init(car.transform);
            GetComponent<NetworkAnimator>().Animator.Rebind();
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

        public override void OnNetworkDespawn()
        {
            if (IsServer && OnPlayerDespawned != null)
            {
                OnPlayerDespawned?.Invoke(this);
            }
        }

        private void Update()
        {
            if (playerName.Value == "")
            {
                return;
            }

            if (transform.GetChildFromName<Transform>("Player_Model") == null)
            {
                UpdateCharacter();
                CustomFunc.ConsoleLog($"{playerName.Value} 설정됨!");
            }
        }
    }
}