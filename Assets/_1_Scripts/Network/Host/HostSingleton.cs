using System;
using Unity.Netcode;
using UnityEngine;

namespace NomadsPlanet
{
    public class HostSingleton : MonoBehaviour
    {
        public static HostSingleton Instance { get; private set; }

        public HostGameManager GameManager { get; private set; }

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

        public void CreateHost(NetworkObject playerPrefab)
        {
            GameManager = new HostGameManager(playerPrefab);
        }

        private void OnDestroy()
        {
            GameManager?.Dispose();
        }
    }
}