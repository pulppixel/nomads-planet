using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Core;

namespace NomadsPlanet
{
    public class ServerSingleton : MonoBehaviour
    {
        private static ServerSingleton instance;

        public ServerGameManager GameManager { get; private set; }

        public static ServerSingleton Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                instance = FindObjectOfType<ServerSingleton>();

                if (instance != null)
                {
                    return instance;
                }

                Debug.LogError("ServerSingleton이 씬에 없습니다!");
                return null;
            }
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