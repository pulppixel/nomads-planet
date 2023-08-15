using System;
using System.Threading.Tasks;
using UnityEngine;

namespace NomadsPlanet
{
    public class ClientSingleton : MonoBehaviour
    {
        public static ClientSingleton Instance { get; private set; }

        public ClientGameManager GameManager { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }

            Instance = this;
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public async Task<bool> CreateClient()
        {
            GameManager = new ClientGameManager();

            return await GameManager.InitAsync();
        }

        private void OnDestroy()
        {
            GameManager?.Dispose();
        }
    }
}