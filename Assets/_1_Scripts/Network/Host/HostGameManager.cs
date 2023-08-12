using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NomadsPlanet.Utils;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
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
        private string _lobbyId;

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

            try
            {
                CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
                lobbyOptions.IsPrivate = false;
                lobbyOptions.Data = new Dictionary<string, DataObject>()
                {
                    {
                        "JoinCode", new DataObject(
                            visibility: DataObject.VisibilityOptions.Member,
                            value: _joinCode
                        )
                    }
                };
                Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(
                    "My Lobby",
                    NetworkSetup.MaxConnections,
                    lobbyOptions
                );

                _lobbyId = lobby.Id;

                HostSingleton.Instance.StartCoroutine(HeartbeatLobby(15));
            }
            catch (LobbyServiceException lobbyServiceException)
            {
                Debug.LogError(lobbyServiceException);
                return;
            }

            NetworkManager.Singleton.StartHost();

            NetworkManager.Singleton.SceneManager.LoadScene(SceneName.GameScene, LoadSceneMode.Single);
        }

        private IEnumerator HeartbeatLobby(float waitTimeSeconds)
        {
            WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);

            while (true)
            {
                Lobbies.Instance.SendHeartbeatPingAsync(_lobbyId);
                yield return delay;
            }
        }
    }
}