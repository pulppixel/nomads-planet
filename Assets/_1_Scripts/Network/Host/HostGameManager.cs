using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NomadsPlanet.Utils;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NomadsPlanet
{
    public class HostGameManager : IDisposable
    {
        private Allocation _allocation;
        private string _joinCode;
        private string _lobbyId;

        public NetworkServer NetworkServer { get; private set; }

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
                        NetworkSetup.JoinCode,
                        new DataObject(
                            visibility: DataObject.VisibilityOptions.Member,
                            value: _joinCode
                        )
                    }
                };

                string playerName = ES3.LoadString(PrefsKey.PlayerNameKey, "Unknown");
                Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(
                    $"{playerName}'s Lobby",
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

            NetworkServer = new NetworkServer(NetworkManager.Singleton);

            UserData userData = new UserData
            {
                userName = ES3.LoadString(PrefsKey.PlayerNameKey, "Missing Name"),
                userAuthId = AuthenticationService.Instance.PlayerId,
                userAvatarType = ES3.Load(PrefsKey.PlayerAvatarKey, (CharacterType)UnityEngine.Random.Range(0, 8)),
            };

            string payload = JsonUtility.ToJson(userData);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
            NetworkManager.Singleton.StartHost();

            NetworkServer.OnClientLeft += HandleClientLeft;

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

        private async void HandleClientLeft(string authId)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_lobbyId, authId);
            }
            catch (LobbyServiceException lobbyServiceException)
            {
                Debug.LogError(lobbyServiceException);
                return;
            }
        }

        public void Dispose()
        {
            Shutdown();
        }

        public async void Shutdown()
        {
            HostSingleton.Instance.StopCoroutine(nameof(HeartbeatLobby));

            if (!string.IsNullOrEmpty(_lobbyId))
            {
                try
                {
                    await Lobbies.Instance.DeleteLobbyAsync(_lobbyId);
                }
                catch (LobbyServiceException lobbyServiceException)
                {
                    Debug.LogError(lobbyServiceException);
                }

                _lobbyId = string.Empty;
            }

            NetworkServer.OnClientLeft -= HandleClientLeft;
            NetworkServer?.Dispose();
        }
    }
}