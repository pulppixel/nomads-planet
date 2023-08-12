using System;
using System.Threading.Tasks;
using NomadsPlanet.Utils;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NomadsPlanet
{
    public class ClientGameManager
    {
        private JoinAllocation _allocation;

        public async Task<bool> InitAsync()
        {
            // Authenticate Player
            await UnityServices.InitializeAsync();

            AuthState authState = await AuthenticationWrapper.DoAuth();

            return authState == AuthState.Authenticated;
        }

        public void GoToMenu()
        {
            SceneManager.LoadScene(SceneName.MenuScene);
        }

        public async Task StartClientAsync(string joinCode)
        {
            try
            {
                _allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            RelayServerData relayServerData = new RelayServerData(_allocation, NetworkSetup.ConnectType);
            transport.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
    }
}