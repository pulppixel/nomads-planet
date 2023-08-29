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

        public UserData UserData { get; private set; }

        public async Task<bool> InitAsync()
        {
            await UnityServices.InitializeAsync();

            _networkClient = new NetworkClient(NetworkManager.Singleton);
            _matchmaker = new MatchplayMatchmaker();

            AuthState authState = await AuthenticationWrapper.DoAuth();

            if (authState == AuthState.Authenticated)
            {
                UserData = new UserData
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

        public void StartClient(string ip, int port)
        {
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData(ip, (ushort)port);
            ConnectClient();
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

        public async Task StartClientAsync(string joinCode)
        {
            try
            {
                _allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
            }
            catch (Exception e)
            {
                CustomFunc.ConsoleLog(e);
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

        public async void MatchmakeAsync(Action<MatchmakerPollingResult> onMatchmakeResponse)
        {
            if (_matchmaker.IsMatchmaking)
            {
                return;
            }

            UserData.userGamePreferences.gameQueue = GameQueue.Solo;
            MatchmakerPollingResult matchResult = await GetMatchAsync();
            onMatchmakeResponse?.Invoke(matchResult);
        }

        private async Task<MatchmakerPollingResult> GetMatchAsync()
        {
            MatchmakingResult matchmakingResult = await _matchmaker.Matchmake(UserData);

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