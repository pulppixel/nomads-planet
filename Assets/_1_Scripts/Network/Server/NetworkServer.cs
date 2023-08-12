using Unity.Netcode;
using UnityEngine;

namespace NomadsPlanet
{
    public class NetworkServer
    {
        private NetworkManager _networkManager;

        public NetworkServer(NetworkManager networkManager)
        {
            _networkManager = networkManager;

            _networkManager.ConnectionApprovalCallback += ApprovalCheck;
        }

        private void ApprovalCheck(
            NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
            string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
            UserData userData = JsonUtility.FromJson<UserData>(payload);

            Debug.Log($"Name: {userData.userName}, Avatar: {(Utils.CharacterType)userData.userAvatarType}");

            response.Approved = true;
        }
    }
}