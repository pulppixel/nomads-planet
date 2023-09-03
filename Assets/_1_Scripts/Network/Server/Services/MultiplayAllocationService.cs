using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NomadsPlanet.Utils;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplay;

namespace NomadsPlanet
{
    public class MultiplayAllocationService : IDisposable
    {
        private IMultiplayService _multiplayService;
        private MultiplayEventCallbacks _serverCallbacks;
        private IServerQueryHandler _serverCheckManager;
        private IServerEvents _serverEvents;
        private CancellationTokenSource _serverCheckCancel;
        private string _allocationId;

        public MultiplayAllocationService()
        {
            try
            {
                _multiplayService = MultiplayService.Instance;
                _serverCheckCancel = new CancellationTokenSource();
            }
            catch (Exception ex)
            {
                CustomFunc.ConsoleLog($"멀티플레이 할당 서비스를 만드는 동안 오류가 발생했습니다.\n{ex}");
            }
        }

        public async Task<MatchmakingResults> SubscribeAndAwaitMatchmakerAllocation()
        {
            if (_multiplayService == null)
            {
                return null;
            }

            _allocationId = null;
            _serverCallbacks = new MultiplayEventCallbacks();
            _serverCallbacks.Allocate += OnMultiplayAllocation;
            _serverEvents = await _multiplayService.SubscribeToServerEventsAsync(_serverCallbacks);

            string allocationID = await AwaitAllocationID();
            MatchmakingResults matchmakingPayload = await GetMatchmakerAllocationPayloadAsync();

            return matchmakingPayload;
        }

        private async Task<string> AwaitAllocationID()
        {
            ServerConfig config = _multiplayService.ServerConfig;
            CustomFunc.ConsoleLog($"Awaiting Allocation. Server Config is:\n" +
                                  $"-ServerID: {config.ServerId}\n" +
                                  $"-AllocationID: {config.AllocationId}\n" +
                                  $"-Port: {config.Port}\n" +
                                  $"-QPort: {config.QueryPort}\n" +
                                  $"-logs: {config.ServerLogDirectory}");

            while (string.IsNullOrEmpty(_allocationId))
            {
                string configID = config.AllocationId;

                if (!string.IsNullOrEmpty(configID) && string.IsNullOrEmpty(_allocationId))
                {
                    CustomFunc.ConsoleLog($"Config had AllocationID: {configID}");
                    _allocationId = configID;
                }

                await Task.Delay(100);
            }

            return _allocationId;
        }

        private async Task<MatchmakingResults> GetMatchmakerAllocationPayloadAsync()
        {
            MatchmakingResults payloadAllocation = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<MatchmakingResults>();
            string modelAsJson = JsonConvert.SerializeObject(payloadAllocation, Formatting.Indented);
            CustomFunc.ConsoleLog(nameof(GetMatchmakerAllocationPayloadAsync) + ":" + Environment.NewLine + modelAsJson);
            return payloadAllocation;
        }

        private void OnMultiplayAllocation(MultiplayAllocation allocation)
        {
            CustomFunc.ConsoleLog($"OnAllocation: {allocation.AllocationId}");

            if (string.IsNullOrEmpty(allocation.AllocationId))
            {
                return;
            }

            _allocationId = allocation.AllocationId;
        }

        public async Task BeginServerCheck()
        {
            if (_multiplayService == null)
            {
                return;
            }

            _serverCheckManager = await _multiplayService.StartServerQueryHandlerAsync(
                    (ushort)20,
                    "ServerName",
                    "",
                    "0", // 이거 언제 빠졌었지...
                    "");

            ServerCheckLoop(_serverCheckCancel.Token);
        }

        public void SetServerName(string name)
        {
            _serverCheckManager.ServerName = name;
        }

        public void SetBuildID(string id)
        {
            _serverCheckManager.BuildId = id;
        }

        public void SetMaxPlayers(ushort players)
        {
            _serverCheckManager.MaxPlayers = players;
        }

        public void AddPlayer()
        {
            _serverCheckManager.CurrentPlayers++;
        }

        public void RemovePlayer()
        {
            _serverCheckManager.CurrentPlayers--;
        }

        public void SetMap(string newMap)
        {
            _serverCheckManager.Map = newMap;
        }

        public void SetMode(string mode)
        {
            _serverCheckManager.GameType = mode;
        }

        private async void ServerCheckLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _serverCheckManager.UpdateServerCheck();
                await Task.Delay(100);
            }
        }

        private static void OnMultiplayDeAllocation(MultiplayDeallocation deallocation)
        {
            CustomFunc.ConsoleLog(
                $"Multiplay Deallocated : ID: {deallocation.AllocationId}\n" +
                $"Event: {deallocation.EventId}\n" +
                $"Server{deallocation.ServerId}");
        }

        private static void OnMultiplayError(MultiplayError error)
        {
            CustomFunc.ConsoleLog($"MultiplayError : {error.Reason}\n{error.Detail}");
        }

        public void Dispose()
        {
            if (_serverCallbacks != null)
            {
                _serverCallbacks.Allocate -= OnMultiplayAllocation;
                _serverCallbacks.Deallocate -= OnMultiplayDeAllocation;
                _serverCallbacks.Error -= OnMultiplayError;
            }

            _serverCheckCancel?.Cancel();
            _serverEvents?.UnsubscribeAsync();
        }
    }
}