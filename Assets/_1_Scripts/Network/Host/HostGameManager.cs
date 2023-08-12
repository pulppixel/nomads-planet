using System;
using System.Threading.Tasks;
using NomadsPlanet.Utils;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NomadsPlanet
{
    public class HostGameManager
    {
        private Allocation _allocation;
        private string _joinCode;

        public async Task StartHostAsync()
        {
            try
            {
                _allocation = await Relay.Instance.CreateAllocationAsync(NetworkSetup.MaxConnections);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }

            try
            {
                _joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
                Debug.Log(_joinCode);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            RelayServerData relayServerData = new RelayServerData(_allocation, NetworkSetup.ConnectType);
            transport.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            NetworkManager.Singleton.SceneManager.LoadScene(SceneName.GameScene, LoadSceneMode.Single);
        }
    }
}