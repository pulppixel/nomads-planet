using Unity.Netcode;
using UnityEngine;

namespace NomadsPlanet
{
    public class HostSingleton : MonoBehaviour
    {
        private static HostSingleton _instance;

        public HostGameManager GameManager { get; private set; }
        
        public static HostSingleton Instance
        {
            get
            {
                if (_instance != null) { return _instance; }

                _instance = FindObjectOfType<HostSingleton>();

                return _instance == null ? null : _instance;
            }
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