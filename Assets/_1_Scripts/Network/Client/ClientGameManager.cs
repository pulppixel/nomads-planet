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

        private UserData _userData;
        private NetworkClient _networkClient;
        private MatchplayMatchmaker _matchmaker;

        public async Task<bool> InitAsync()
        {
            await UnityServices.InitializeAsync();

            _networkClient = new NetworkClient(NetworkManager.Singleton);
            _matchmaker = new MatchplayMatchmaker();

            AuthState authState = await AuthenticationWrapper.DoAuth();

            if (authState == AuthState.Authenticated)
            {
                _userData = new UserData
                {
                    userName = ES3.LoadString(PrefsKey.NameKey, "Missing Name"),
                    userCarType = ES3.Load(PrefsKey.CarTypeKey, Random.Range(0, 8)),
                    userAvatarType = ES3.Load(PrefsKey.AvatarTypeKey, Random.Range(0, 8)),
                    userAuthId = AuthenticationService.Instance.PlayerId
                };
                return true;
            }

            return false;
        }

        public static void GoToMenu()
        {
            SceneManager.LoadScene(SceneName.MenuScene);
        }

        private void StartClient(string ip, int port)
        {
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData(ip, (ushort)port);
            ConnectClient();
        }

        public void UpdateUserData(string userName = "", int userCarType = -1, int userAvatarType = -1)
        {
            _userData.userName = userName == "" ? _userData.userName : userName;
            _userData.userCarType = userCarType == -1 ? _userData.userCarType : userCarType;
            _userData.userAvatarType = userAvatarType == -1 ? _userData.userAvatarType : userAvatarType;

            string payload = JsonUtility.ToJson(_userData);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
        }

        public async Task<bool> StartClientAsync(string joinCode)
        {
            try
            {
                _allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
            }
            catch (Exception e)
            {
                CustomFunc.ConsoleLog(e);
                return false;
            }

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            RelayServerData relayServerData = new RelayServerData(_allocation, NetworkSetup.ConnectType);
            transport.SetRelayServerData(relayServerData);

            ConnectClient();
            return true;
        }

        private void ConnectClient()
        {
            string payload = JsonUtility.ToJson(_userData);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
            NetworkManager.Singleton.StartClient();
        }

        public async void MatchmakeAsync(Action<MatchmakerPollingResult> onMatchmakeResponse)
        {
            if (_matchmaker.IsMatchmaking)
            {
                return;
            }

            MatchmakerPollingResult matchResult = await GetMatchAsync();
            onMatchmakeResponse?.Invoke(matchResult);
        }

        private async Task<MatchmakerPollingResult> GetMatchAsync()
        {
            MatchmakingResult matchmakingResult = await _matchmaker.Matchmake(_userData);

            if (matchmakingResult.Result == MatchmakerPollingResult.Success)
            {
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