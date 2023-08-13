using Unity.Netcode;
using UnityEngine.SceneManagement;
using NomadsPlanet.Utils;

namespace NomadsPlanet
{
    public class NetworkClient
    {
        private readonly NetworkManager _networkManager;

        public NetworkClient(NetworkManager networkManager)
        {
            _networkManager = networkManager;

            _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
        }

        private void OnClientDisconnect(ulong clientId)
        {
            if (clientId != 0 && clientId != _networkManager.LocalClientId)
            {
                return;
            }

            if (SceneManager.GetActiveScene().name != SceneName.MenuScene)
            {
                SceneManager.LoadScene(SceneName.MenuScene);
            }

            if (_networkManager.IsConnectedClient)
            {
                _networkManager.Shutdown();
            }
        }
    }
}