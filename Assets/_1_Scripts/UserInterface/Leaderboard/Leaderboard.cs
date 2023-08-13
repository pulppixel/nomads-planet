using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

namespace NomadsPlanet
{
    public class Leaderboard : NetworkBehaviour
    {
        [SerializeField] private Transform leaderboardEntityHolder;
        [SerializeField] private LeaderboardEntityDisplay leaderboardEntityPrefab;

        private NetworkList<LeaderboardEntityState> _leaderboardEntities;
        private List<LeaderboardEntityDisplay> _entityDisplays = new();

        private void Awake()
        {
            _leaderboardEntities = new NetworkList<LeaderboardEntityState>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                _leaderboardEntities.OnListChanged += HandleLeaderboardEntitiesChanged;

                foreach (LeaderboardEntityState entity in _leaderboardEntities)
                {
                    HandleLeaderboardEntitiesChanged(new NetworkListEvent<LeaderboardEntityState>
                    {
                        Type = NetworkListEvent<LeaderboardEntityState>.EventType.Add,
                        Value = entity,
                    });
                }
            }

            if (IsServer)
            {
                DrivingPlayer[] players = FindObjectsByType<DrivingPlayer>(FindObjectsSortMode.None);

                foreach (DrivingPlayer player in players)
                {
                    HandlePlayerSpawned(player);
                }

                DrivingPlayer.OnPlayerSpawned += HandlePlayerSpawned;
                DrivingPlayer.OnPlayerDespawned += HandlePlayerDespawned;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsClient)
            {
                _leaderboardEntities.OnListChanged -= HandleLeaderboardEntitiesChanged;
            }

            if (IsServer)
            {
                DrivingPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
                DrivingPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
            }
        }

        private void HandleLeaderboardEntitiesChanged(NetworkListEvent<LeaderboardEntityState> changeEvent)
        {
            switch (changeEvent.Type)
            {
                case NetworkListEvent<LeaderboardEntityState>.EventType.Add:
                    if (_entityDisplays.All(x => x.ClientId != changeEvent.Value.ClientId))
                    {
                        var leaderboardEntity = Instantiate(leaderboardEntityPrefab, leaderboardEntityHolder);
                        leaderboardEntity.Initialize(changeEvent.Value.ClientId,
                            changeEvent.Value.PlayerName,
                            changeEvent.Value.CharacterType,
                            changeEvent.Value.Coins
                        );
                        _entityDisplays.Add(leaderboardEntity);
                    }

                    break;
                case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                    var displayToRemove = _entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);

                    if (displayToRemove != null)
                    {
                        displayToRemove.transform.SetParent(null);
                        Destroy(displayToRemove.gameObject);
                        _entityDisplays.Remove(displayToRemove);
                    }

                    break;
                case NetworkListEvent<LeaderboardEntityState>.EventType.Value:
                    var displayToUpdate = _entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);

                    if (displayToUpdate != null)
                    {
                        displayToUpdate.UpdateCoins(changeEvent.Value.Coins);
                    }

                    break;
            }
        }

        private void HandlePlayerSpawned(DrivingPlayer drivingPlayer)
        {
            _leaderboardEntities.Add(new LeaderboardEntityState
            {
                ClientId = drivingPlayer.OwnerClientId,
                PlayerName = drivingPlayer.playerName.Value,
                CharacterType = drivingPlayer.characterType.Value,
                Coins = 0,
            });

            drivingPlayer.Wallet.totalCoins.OnValueChanged += (oldCoins, newCoins) =>
                HandleCoinsChanged(drivingPlayer.OwnerClientId, newCoins);
        }

        private void HandlePlayerDespawned(DrivingPlayer drivingPlayer)
        {
            if (_leaderboardEntities == null)
            {
                return;
            }

            foreach (LeaderboardEntityState entity in _leaderboardEntities)
            {
                if (entity.ClientId != drivingPlayer.OwnerClientId)
                {
                    continue;
                }

                _leaderboardEntities.Remove(entity);
                break;
            }

            drivingPlayer.Wallet.totalCoins.OnValueChanged -= (oldCoins, newCoins) =>
                HandleCoinsChanged(drivingPlayer.OwnerClientId, newCoins);
        }

        private void HandleCoinsChanged(ulong clientId, int newCoins)
        {
            for (int i = 0; i < _leaderboardEntities.Count; i++)
            {
                if (_leaderboardEntities[i].ClientId != clientId)
                {
                    continue;
                }

                _leaderboardEntities[i] = new LeaderboardEntityState
                {
                    ClientId = _leaderboardEntities[i].ClientId,
                    PlayerName = _leaderboardEntities[i].PlayerName,
                    Coins = newCoins,
                };

                return;
            }
        }
    }
}