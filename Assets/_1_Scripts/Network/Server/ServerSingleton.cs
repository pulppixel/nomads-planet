using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Core;

namespace NomadsPlanet
{
    public class ServerSingleton : MonoBehaviour
    {
        public static ServerSingleton Instance { get; private set; }

        public ServerGameManager GameManager { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public async Task CreateServer(NetworkObject playerPrefab)
        {
            await UnityServices.InitializeAsync();

            GameManager = new ServerGameManager(
                ApplicationData.IP(),
                ApplicationData.Port(),
                ApplicationData.QPort(),
                NetworkManager.Singleton,
                playerPrefab
            );
        }

        private void OnDestroy()
        {
            GameManager?.Dispose();
        }
    }
}