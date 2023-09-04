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

        private bool isSetupDone;

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

            if (!IsServer)
            {
                return;
            }

            UserData userData;
            isSetupDone = false;

            if (IsHost)
            {
                userData = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            }
            else
            {
                userData = ServerSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            }

            playerName.Value = userData.userName;
            avatarType.Value = userData.userAvatarType;
            carType.Value = userData.userCarType;
            OnPlayerSpawned?.Invoke(this);
        }

        private void UpdateCharacter()
        {
            var avatar = Instantiate(playerPrefabs[avatarType.Value], transform);
            avatar.transform.SetSiblingIndex(2);
            avatar.gameObject.name = "Player_Model";

            var car = Instantiate(carPrefabs[carType.Value], transform);
            car.transform.SetSiblingIndex(3);
            car.gameObject.name = "Player_Car";

            CarController.Init(car.transform);
            GetComponent<NetworkAnimator>().Animator.Rebind();
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
            if (isSetupDone || playerName.Value == "")
            {
                return;
            }

            if (transform.GetChildFromName<Transform>("Player_Model") == null)
            {
                UpdateCharacter();
                isSetupDone = true;
            }
        }
    }
}