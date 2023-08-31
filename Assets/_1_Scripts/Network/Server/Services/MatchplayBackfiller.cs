using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using NomadsPlanet.Utils;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;

namespace NomadsPlanet
{
    public class MatchplayBackfiller : IDisposable
    {
        private readonly CreateBackfillTicketOptions _createBackfillOptions;
        private BackfillTicket _localBackfillTicket;

        private bool _localDataDirty;
        private readonly int _maxPlayers;
        private const int TicketCheckMs = 1000;

        private int MatchPlayerCount => _localBackfillTicket?.Properties.MatchProperties.Players.Count ?? 0;

        private MatchProperties MatchProperties => _localBackfillTicket.Properties.MatchProperties;
        public bool IsBackfilling { get; private set; }

        public MatchplayBackfiller(string connection, string queueName, MatchProperties matchmakerPayloadProperties, int maxPlayers)
        {
            _maxPlayers = maxPlayers;
            BackfillTicketProperties backfillProperties = new BackfillTicketProperties(matchmakerPayloadProperties);
            _localBackfillTicket = new BackfillTicket
            {
                Id = matchmakerPayloadProperties.BackfillTicketId,
                Properties = backfillProperties
            };

            _createBackfillOptions = new CreateBackfillTicketOptions
            {
                Connection = connection,
                QueueName = queueName,
                Properties = backfillProperties
            };
        }

        public async Task BeginBackfilling()
        {
            if (IsBackfilling)
            {
                CustomFunc.ConsoleLog("이미 Backfill이 완료되었으므로, 다시 시작할 필요가 없습니다.");
                return;
            }

            CustomFunc.ConsoleLog($"backfill 서버 시작: {MatchPlayerCount}/{_maxPlayers}");

            if (string.IsNullOrEmpty(_localBackfillTicket.Id))
            {
                _localBackfillTicket.Id = await MatchmakerService.Instance.CreateBackfillTicketAsync(_createBackfillOptions);
            }

            IsBackfilling = true;
            BackfillLoop();
        }

        public void AddPlayerToMatch(UserData userData)
        {
            if (!IsBackfilling)
            {
                CustomFunc.ConsoleLog("Backfill 티켓이 만들어지기 전에 사용자를 Backfill 티켓에 추가할 수 없습니다.");
                return;
            }

            if (GetPlayerById(userData.userAuthId) != null)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("User: {0} - {1} already in Match. Ignoring add.",
                    userData.userName,
                    userData.userAuthId);
#endif
                return;
            }

            Player matchmakerPlayer = new Player(userData.userAuthId, userData.userGamePreferences);

            MatchProperties.Players.Add(matchmakerPlayer);
            MatchProperties.Teams[0].PlayerIds.Add(matchmakerPlayer.Id);
            _localDataDirty = true;
        }

        public int RemovePlayerFromMatch(string userId)
        {
            Player playerToRemove = GetPlayerById(userId);
            if (playerToRemove == null)
            {
                CustomFunc.ConsoleLog($"로컬 Backfill 데이터에 ID: {userId}의 사용자가 없습니다.");
                return MatchPlayerCount;
            }

            MatchProperties.Players.Remove(playerToRemove);
            MatchProperties.Teams[0].PlayerIds.Remove(userId);
            _localDataDirty = true;

            return MatchPlayerCount;
        }

        public bool NeedsPlayers()
        {
            return MatchPlayerCount < _maxPlayers;
        }

        private Player GetPlayerById(string userId)
        {
            return MatchProperties.Players.FirstOrDefault(
                p => p.Id.Equals(userId));
        }

        public async Task StopBackfill()
        {
            if (!IsBackfilling)
            {
                CustomFunc.ConsoleLog("시작하기 전에 Backfill을 멈출 수는 없습니다.", true);
                return;
            }

            await MatchmakerService.Instance.DeleteBackfillTicketAsync(_localBackfillTicket.Id);
            IsBackfilling = false;
            _localBackfillTicket.Id = null;
        }

        private async void BackfillLoop()
        {
            while (IsBackfilling)
            {
                if (_localDataDirty)
                {
                    await MatchmakerService.Instance.UpdateBackfillTicketAsync(_localBackfillTicket.Id, _localBackfillTicket);
                    _localDataDirty = false;
                }
                else
                {
                    _localBackfillTicket = await MatchmakerService.Instance.ApproveBackfillTicketAsync(_localBackfillTicket.Id);
                }

                if (!NeedsPlayers())
                {
                    await StopBackfill();
                    break;
                }

                await Task.Delay(TicketCheckMs);
            }
        }

        public void Dispose()
        {
            _ = StopBackfill();
        }
    }
}