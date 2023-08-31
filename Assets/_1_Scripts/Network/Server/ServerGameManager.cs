#if UNITY_ANDROID || UNITY_IOS
#else
using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using NomadsPlanet.Utils;

namespace NomadsPlanet
{
    public class ServerGameManager : IDisposable
    {
        private readonly string _serverIP;
        private readonly int _serverPort;
        private readonly int _queryPort;
        private MatchplayBackfiller _backfiller;
        private readonly MultiplayAllocationService _multiplayAllocationService;
        public NetworkServer NetworkServer { get; private set; }

        public ServerGameManager(string serverIP, int serverPort,
            int queryPort, NetworkManager manager, NetworkObject playerPrefab)
        {
            _serverIP = serverIP;
            _serverPort = serverPort;
            _queryPort = queryPort;
            NetworkServer = new NetworkServer(manager, playerPrefab);
            _multiplayAllocationService = new MultiplayAllocationService();
        }

        public async Task StartGameServerAsync()
        {
            await _multiplayAllocationService.BeginServerCheck();

            try
            {
                MatchmakingResults matchmakerPayload = await GetMatchmakerPayload();

                if (matchmakerPayload != null)
                {
                    await StartBackfill(matchmakerPayload);
                    NetworkServer.OnUserJoined += UserJoined;
                    NetworkServer.OnUserLeft += UserLeft;
                }
                else
                {
                    CustomFunc.ConsoleLog("매치메이커 페이로드 시간이 초과되었습니다.");
                }
            }
            catch (Exception e)
            {
                CustomFunc.ConsoleLog(e);
                return;
            }

            if (!NetworkServer.OpenConnection(_serverIP, _serverPort))
            {
                CustomFunc.ConsoleLog("네트워크 서버가 예상대로 시작되지 않았습니다.");
                return;
            }
        }

        private async Task<MatchmakingResults> GetMatchmakerPayload()
        {
            Task<MatchmakingResults> matchmakerPayloadTask =
                _multiplayAllocationService.SubscribeAndAwaitMatchmakerAllocation();

            if (await Task.WhenAny(matchmakerPayloadTask, Task.Delay(20000)) == matchmakerPayloadTask)
            {
                return matchmakerPayloadTask.Result;
            }

            return null;
        }

        private async Task StartBackfill(MatchmakingResults payload)
        {
            _backfiller = new MatchplayBackfiller(
                $"{_serverIP}:{_serverPort}",
                payload.QueueName,
                payload.MatchProperties,
                NetworkSetup.MaxConnections
            );

            if (_backfiller.NeedsPlayers())
            {
                await _backfiller.BeginBackfilling();
            }
        }

        private void UserJoined(UserData user)
        {
            _backfiller.AddPlayerToMatch(user);
            _multiplayAllocationService.AddPlayer();

            if (!_backfiller.NeedsPlayers() && _backfiller.IsBackfilling)
            {
                _ = _backfiller.StopBackfill();
            }
        }

        private void UserLeft(UserData user)
        {
            int playerCount = _backfiller.RemovePlayerFromMatch(user.userAuthId);
            _multiplayAllocationService.RemovePlayer();

            if (playerCount <= 0)
            {
                CloseServer();
                return;
            }

            if (_backfiller.NeedsPlayers() && !_backfiller.IsBackfilling)
            {
                _ = _backfiller.BeginBackfilling();
            }
        }

        private async void CloseServer()
        {
            await _backfiller.StopBackfill();
            Dispose();
            Application.Quit();
        }

        public void Dispose()
        {
            NetworkServer.OnUserJoined -= UserJoined;
            NetworkServer.OnUserLeft -= UserLeft;

            _backfiller?.Dispose();
            _multiplayAllocationService?.Dispose();
            NetworkServer?.Dispose();
        }
    }
}
#endif