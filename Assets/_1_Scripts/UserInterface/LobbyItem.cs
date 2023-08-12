using System;
using NomadsPlanet.Utils;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace NomadsPlanet
{
    public class LobbyItem : MonoBehaviour
    {
        private TMP_Text _lobbyNameText;
        private TMP_Text _lobbyPlayersText;

        private LobbiesList _lobbiesList;
        private Lobby _lobby;

        private void Awake()
        {
            _lobbyNameText = transform.GetChildFromName<TMP_Text>("LobbyNameText");
            _lobbyPlayersText = transform.GetChildFromName<TMP_Text>("LobbyPlayersText");
        }

        public void Initialize(LobbiesList lobbiesList, Lobby lobby)
        {
            _lobbiesList = lobbiesList;
            _lobby = lobby;
            
            _lobbyNameText.text = lobby.Name;
            _lobbyPlayersText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
        }

        public void Join()
        {
            _lobbiesList.JoinAsync(_lobby);
        }
    }
}