using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace NomadsPlanet
{
    public class LobbiesList : MonoBehaviour
    {
        [SerializeField] private MainMenu mainMenu;
        [SerializeField] private Transform lobbyItemParent;
        [SerializeField] private LobbyItem lobbyItemPrefab;

        private bool _isRefreshing;

        private void OnEnable()
        {
            RefreshList();
        }

        public async void RefreshList()
        {
            if (_isRefreshing)
            {
                return;
            }

            _isRefreshing = true;

            try
            {
                QueryLobbiesOptions options = new QueryLobbiesOptions
                {
                    Count = 25,
                    Filters = new List<QueryFilter>
                    {
                        new(
                            field: QueryFilter.FieldOptions.AvailableSlots,
                            op: QueryFilter.OpOptions.GT,
                            value: "0"
                        ),
                        new(
                            field: QueryFilter.FieldOptions.IsLocked,
                            op: QueryFilter.OpOptions.EQ,
                            value: "0"
                        ),
                    }
                };

                QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

                foreach (Transform child in lobbyItemParent)
                {
                    Destroy(child.gameObject);
                }

                foreach (Lobby lobby in lobbies.Results)
                {
                    LobbyItem lobbyItem = Instantiate(lobbyItemPrefab, lobbyItemParent);
                    lobbyItem.Initialize(this, lobby);
                }
            }
            catch (LobbyServiceException lobbyServiceException)
            {
                Debug.LogError(lobbyServiceException);
            }

            _isRefreshing = false;
        }


        public void JoinAsync(Lobby lobby)
        {
            mainMenu.JoinAsync(lobby);
        }
    }
}