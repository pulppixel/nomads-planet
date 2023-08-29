using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace NomadsPlanet
{
    public class Leaderboard : NetworkBehaviour
    {
        [SerializeField] private Transform leaderboardEntityHolder;
        [SerializeField] private LeaderboardEntityDisplay leaderboardEntityPrefab;
        [SerializeField] private int entitiesToDisplay = 8;

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
            if (!gameObject.scene.isLoaded)
            {
                return;
            }

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
                    var displayToRemove = _entityDisplays.FirstOrDefault(x =>
                        x.ClientId == changeEvent.Value.ClientId
                    );

                    if (displayToRemove != null)
                    {
                        displayToRemove.transform.SetParent(null);
                        Destroy(displayToRemove.gameObject);
                        _entityDisplays.Remove(displayToRemove);
                    }

                    break;
                case NetworkListEvent<LeaderboardEntityState>.EventType.Value:
                    var displayToUpdate = _entityDisplays.FirstOrDefault(x =>
                        x.ClientId == changeEvent.Value.ClientId
                    );

                    if (displayToUpdate != null)
                    {
                        displayToUpdate.UpdateCoins(changeEvent.Value.Coins);
                    }

                    break;
            }

            _entityDisplays.Sort((x, y) => y.Coins.CompareTo(x.Coins)
            );

            for (int i = 0; i < _entityDisplays.Count; i++)
            {
                _entityDisplays[i].transform.SetSiblingIndex(i);
                _entityDisplays[i].UpdateDisplays();
                // should show
                _entityDisplays[i].gameObject.SetActive(i <= entitiesToDisplay - 1);
            }

            LeaderboardEntityDisplay myDisplay = GetClientDisplay();

            if (myDisplay != null)
            {
                if (myDisplay.transform.GetSiblingIndex() >= entitiesToDisplay)
                {
                    leaderboardEntityHolder.GetChild(entitiesToDisplay - 1).gameObject.SetActive(false);
                    myDisplay.gameObject.SetActive(true);
                }
            }
        }

        public LeaderboardEntityDisplay GetClientDisplay()
        {
            return _entityDisplays.FirstOrDefault(x =>
                x.ClientId == NetworkManager.Singleton.LocalClientId
            );
        }

        private void HandlePlayerSpawned(DrivingPlayer drivingPlayer)
        {
            _leaderboardEntities.Add(new LeaderboardEntityState
            {
                ClientId = drivingPlayer.OwnerClientId,
                PlayerName = drivingPlayer.playerName.Value,
                CharacterType = drivingPlayer.avatarType.Value,
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
                    CharacterType = _leaderboardEntities[i].CharacterType,
                    Coins = newCoins,
                };

                return;
            }
        }
    }
}