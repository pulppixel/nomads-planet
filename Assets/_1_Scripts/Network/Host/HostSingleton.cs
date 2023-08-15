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
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = FindObjectOfType<HostSingleton>();

                if (_instance != null)
                {
                    return _instance;
                }

                Debug.LogError("HostSingleton이 씬에 없습니다!");
                return null;
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