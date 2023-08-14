using System;
using System.Text;
using System.Threading.Tasks;
using NomadsPlanet.Utils;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace NomadsPlanet
{
    public class ClientGameManager : IDisposable
    {
        private JoinAllocation _allocation;
        private NetworkClient _networkClient;
        private MatchplayMatchmaker _matchmaker;
        private UserData _userData;

        public async Task<bool> InitAsync()
        {
            // Authenticate Player
            await UnityServices.InitializeAsync();

            _networkClient = new NetworkClient(NetworkManager.Singleton);
            _matchmaker = new MatchplayMatchmaker();

            AuthState authState = await AuthenticationWrapper.DoAuth();

            if (authState == AuthState.Authenticated)
            {
                _userData = new UserData
                {
                    userName = ES3.LoadString(PrefsKey.PlayerNameKey, "Missing Name"),
                    userAuthId = AuthenticationService.Instance.PlayerId,
                    userCarType = ES3.Load(PrefsKey.PlayerCarKey, (CarType)Random.Range(0, 8)),
                    userAvatarType = ES3.Load(PrefsKey.PlayerAvatarKey, (CharacterType)Random.Range(0, 8)),
                };

                return true;
            }

            return false;
        }

        public void GoToMenu()
        {
            SceneManager.LoadScene(SceneName.MenuScene);
        }

        public void StartClient(string ip, int port)
        {
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData(ip, (ushort)port);


            ConnectClient();
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

            ConnectClient();
        }

        private void ConnectClient()
        {
            string payload = JsonUtility.ToJson(_userData);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
            NetworkManager.Singleton.StartClient();
        }

        public async void MatchmakeAsync(Action<MatchmakerPollingResult> onMatchMakeResponse)
        {
            if (_matchmaker.IsMatchmaking)
            {
                return;
            }

            MatchmakerPollingResult matchResult = await GetMatchAsync();
            onMatchMakeResponse?.Invoke(matchResult);
        }

        private async Task<MatchmakerPollingResult> GetMatchAsync()
        {
            MatchmakingResult matchmakingResult = await _matchmaker.Matchmake(_userData);

            if (matchmakingResult.Result == MatchmakerPollingResult.Success)
            {
                // 서버에 연결
                StartClient(matchmakingResult.IP, matchmakingResult.Port);
            }

            return matchmakingResult.Result;
        }

        public async Task CancelMatchmaking()
        {
            await _matchmaker.CancelMatchmaking();
        }

        public void Disconnect()
        {
            _networkClient.Disconnect();
        }

        public void Dispose()
        {
            _networkClient?.Dispose();
        }
    }
}