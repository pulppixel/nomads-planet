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
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace NomadsPlanet
{
    public class ClientGameManager : IDisposable
    {
        public UserData UserData { get; private set; }
        private JoinAllocation _allocation;
        private NetworkClient _networkClient;
        private MatchplayMatchmaker _matchmaker;

        public async Task<bool> InitAsync()
        {
            // Authenticate Player
            await UnityServices.InitializeAsync();

            _networkClient = new NetworkClient(NetworkManager.Singleton);
            _matchmaker = new MatchplayMatchmaker();

            AuthState authState = await AuthenticationWrapper.DoAuth();

            if (authState == AuthState.Authenticated)
            {
                UserData = new UserData
                {
                    userName = ES3.LoadString(PrefsKey.NameKey, "Missing Name"),
                    userAuthId = AuthenticationService.Instance.PlayerId,
                    userCarType = ES3.Load(PrefsKey.CarTypeKey, Random.Range(0, 8)),
                    userAvatarType = ES3.Load(PrefsKey.AvatarTypeKey, Random.Range(0, 8)),
                };

                return true;
            }

            return false;
        }

        public static void GoToMenu()
        {
            SceneManager.LoadScene(SceneName.MenuScene);
        }

        public void UpdateUserData(string userName = "", int userCarType = -1, int userAvatarType = -1)
        {
            UserData.userName = userName == "" ? UserData.userName : userName;
            UserData.userCarType = userCarType == -1 ? UserData.userCarType : userCarType;
            UserData.userAvatarType = userAvatarType == -1 ? UserData.userAvatarType : userAvatarType;

            string payload = JsonUtility.ToJson(UserData);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
        }

        private void StartClient(string ip, int port)
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
                CustomFunc.ConsoleLog(e, true);
                return;
            }

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            RelayServerData relayServerData = new RelayServerData(_allocation, NetworkSetup.ConnectType);
            transport.SetRelayServerData(relayServerData);

            ConnectClient();
        }

        private void ConnectClient()
        {
            string payload = JsonUtility.ToJson(UserData);
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
            MatchmakingResult matchmakingResult = await _matchmaker.Matchmake(UserData);

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