﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NomadsPlanet.Utils;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace NomadsPlanet
{
    public class NetworkServer : IDisposable
    {
        private NetworkManager _networkManager;
        private NetworkObject _playerPrefab;

        public Action<UserData> OnUserJoined;
        public Action<UserData> OnUserLeft;

        public Action<string> OnClientLeft;

        private readonly Dictionary<ulong, string> _clientIdToAuth = new Dictionary<ulong, string>();
        private readonly Dictionary<string, UserData> _authIdToUserData = new Dictionary<string, UserData>();

        public NetworkServer(NetworkManager networkManager, NetworkObject playerPrefab)
        {
            _networkManager = networkManager;
            _playerPrefab = playerPrefab;

            _networkManager.ConnectionApprovalCallback += ApprovalCheck;
            _networkManager.OnServerStarted += OnNetworkReady;
        }

        public bool OpenConnection(string ip, int port)
        {
            UnityTransport transport = _networkManager.gameObject.GetComponent<UnityTransport>();
            transport.SetConnectionData(ip, (ushort)port);
            return _networkManager.StartServer();
        }

        private void ApprovalCheck(
            NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
            string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
            UserData userData = JsonUtility.FromJson<UserData>(payload);

            _clientIdToAuth[request.ClientNetworkId] = userData.userAuthId;
            _authIdToUserData[userData.userAuthId] = userData;
            OnUserJoined?.Invoke(userData);

            _ = SpawnPlayerDelayed(request.ClientNetworkId);

            response.Approved = true;
            response.CreatePlayerObject = false;
        }

        private async Task SpawnPlayerDelayed(ulong clientId)
        {
            await Task.Delay(3000);

            NetworkObject playerInstance = GameObject.Instantiate(
                _playerPrefab,
                SpawnPoint.GetRandomSpawnPos(),
                quaternion.identity
            );

            CustomFunc.ConsoleLog($"{clientId} 생성됨!!\n" +
                                  $"{playerInstance.name}\n" +
                                  $"{playerInstance.transform.position}");

            playerInstance.SpawnAsPlayerObject(clientId);
        }

        private void OnNetworkReady()
        {
            _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
        }

        private void OnClientDisconnect(ulong clientId)
        {
            if (_clientIdToAuth.TryGetValue(clientId, out string authId))
            {
                _clientIdToAuth.Remove(clientId);
                OnUserLeft?.Invoke(_authIdToUserData[authId]);
                _authIdToUserData.Remove(authId);
                OnClientLeft?.Invoke(authId);
            }
        }

        public UserData GetUserDataByClientId(ulong clientId)
        {
            if (_clientIdToAuth.TryGetValue(clientId, out string authId))
            {
                if (_authIdToUserData.TryGetValue(authId, out UserData data))
                {
                    return data;
                }

                return null;
            }

            return null;
        }

        public void Dispose()
        {
            if (_networkManager == null)
            {
                return;
            }

            _networkManager.ConnectionApprovalCallback -= ApprovalCheck;
            _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
            _networkManager.OnServerStarted -= OnNetworkReady;

            if (_networkManager.IsListening)
            {
                _networkManager.Shutdown();
            }
        }
    }
}