using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace NomadsPlanet
{
    public class NetworkServer : IDisposable
    {
        private readonly NetworkManager _networkManager;

        public Action<UserData> OnUserJoined;
        public Action<UserData> OnUserLeft;

        public Action<string> OnClientLeft;

        private readonly Dictionary<ulong, string> _clientIdToAuth = new Dictionary<ulong, string>();
        private readonly Dictionary<string, UserData> _authIdToUserData = new Dictionary<string, UserData>();

        public NetworkServer(NetworkManager networkManager)
        {
            _networkManager = networkManager;

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

            response.Approved = true;
            response.Position = SpawnPoint.GetRandomSpawnPos();
            response.Rotation = Quaternion.identity;
            response.CreatePlayerObject = true;
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