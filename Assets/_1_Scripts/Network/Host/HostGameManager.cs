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
        private readonly NetworkObject _playerPrefab;

        private string _joinCode;
        private string _lobbyId;

        public NetworkServer NetworkServer { get; private set; }
        public string JoinCode => _joinCode;

        public HostGameManager(NetworkObject playerPrefab)
        {
            _playerPrefab = playerPrefab;
        }

        public async Task StartHostAsync()
        {
            try
            {
                _allocation = await Relay.Instance.CreateAllocationAsync(NetworkSetup.MaxConnections);
            }
            catch (Exception e)
            {
                CustomFunc.ConsoleLog(e, true);
                return;
            }

            try
            {
                _joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
                CustomFunc.ConsoleLog(_joinCode);
            }
            catch (Exception e)
            {
                CustomFunc.ConsoleLog(e, true);
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
                CustomFunc.ConsoleLog(lobbyServiceException, true);
                return;
            }

            NetworkServer = new NetworkServer(NetworkManager.Singleton, _playerPrefab);

            UserData userData = new UserData
            {
                userName = ES3.LoadString(PrefsKey.PlayerNameKey, "Missing Name"),
                userAuthId = AuthenticationService.Instance.PlayerId,
                userCarType = ES3.Load(PrefsKey.PlayerCarKey, (CarType)UnityEngine.Random.Range(0, 8)),
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
                CustomFunc.ConsoleLog(lobbyServiceException, true);
                return;
            }
        }

        public void Dispose()
        {
            Shutdown();
        }

        public async void Shutdown()
        {
            if (string.IsNullOrEmpty(_lobbyId))
            {
                return;
            }

            HostSingleton.Instance.StopCoroutine(nameof(HeartbeatLobby));

            try
            {
                await Lobbies.Instance.DeleteLobbyAsync(_lobbyId);
            }
            catch (LobbyServiceException lobbyServiceException)
            {
                CustomFunc.ConsoleLog(lobbyServiceException, true);
            }

            _lobbyId = string.Empty;

            NetworkServer.OnClientLeft -= HandleClientLeft;

            NetworkServer?.Dispose();
        }
    }
}