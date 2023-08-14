using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NomadsPlanet.Utils;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NomadsPlanet
{
    public class ServerGameManager : IDisposable
    {
        private string _serverIP;
        private int _serverPort;
        private int _queryPort;
        private NetworkServer _networkServer;
        private MultiplayAllocationService _multiplayAllocationService;

        public ServerGameManager(string serverIP, int serverPort, int queryPort, NetworkManager manager)
        {
            _serverIP = serverIP;
            _serverPort = serverPort;
            _queryPort = queryPort;
            _networkServer = new NetworkServer(manager);
            _multiplayAllocationService = new MultiplayAllocationService();
        }

        public async Task StartGameServerAsync()
        {
            await _multiplayAllocationService.BeginServerCheck();

            if (!_networkServer.OpenConnection(_serverIP, _serverPort))
            {
                Debug.LogError("네트워크 서버가 시작되지 않았다.");
                return;
            }

            NetworkManager.Singleton.SceneManager.LoadScene(SceneName.GameScene, LoadSceneMode.Single);
        }

        public void Dispose()
        {
            _multiplayAllocationService?.Dispose();
            _networkServer?.Dispose();
        }
    }
}