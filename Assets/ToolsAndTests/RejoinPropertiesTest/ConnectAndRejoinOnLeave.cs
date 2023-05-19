// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectAndJoinRandom.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities, 
// </copyright>
// <summary>
//  Simple component to call ConnectUsingSettings and to get into a PUN room easily.
// </summary>
// <remarks>
//  A custom inspector provides a button to connect in PlayMode, should AutoConnect be false.
//  </remarks>                                                                                               
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------


using ExitGames.Client.Photon;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

namespace Photon.Pun.UtilityScripts
{
    
    public class ConnectAndRejoinOnLeave : MonoBehaviourPunCallbacks
    {
        /// <summary>Connect automatically? If false you can set this to true later on or call ConnectUsingSettings in your own scripts.</summary>
        public bool AutoConnect = true;

        /// <summary>Used as PhotonNetwork.GameVersion.</summary>
        public byte Version = 1;


        public void Start()
        {
            if (this.AutoConnect)
            {
                this.ConnectNow();
            }
        }

        public void ConnectNow()
        {

            Debug.Log("ConnectAndRejoinOnLeave.ConnectNow() will now call: PhotonNetwork.ConnectUsingSettings().");
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = this.Version + "." + SceneManagerHelper.ActiveSceneBuildIndex;
            PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { {"prop1","initial join"} });
        }
        public void LeaveNow()
        {
            Debug.Log("ConnectAndRejoinOnLeave.LeaveNow() will now call: PhotonNetwork.LeaveRoom().");
            PhotonNetwork.LeaveRoom();
        }


        // below, we implement some callbacks of the Photon Realtime API.
        // Being a MonoBehaviourPunCallbacks means, we can override the few methods which are needed here.


        public override void OnConnectedToMaster()
        {
            if (!string.IsNullOrEmpty(cachedRoomName))
            {
                Debug.Log("OnConnectedToMaster() ReJoining.");
                //PhotonNetwork.LocalPlayer.CustomProperties = new Hashtable();
                PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { "prop1", null } });
                PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { "prop2", "ReJoined" } });
                PhotonNetwork.RejoinRoom(this.cachedRoomName);
            }
            else
            {
                Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("OnJoinedLobby(). This client is connected and does get a room-list. This script now calls: PhotonNetwork.JoinRandomRoom();");
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 4, PlayerTtl = 10000}, null);
        }

        // the following methods are implemented to give you some context. re-implement them as needed.
        public override void OnDisconnected(DisconnectCause cause)
        {
			Debug.LogError("OnDisconnected("+cause+")");
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running.");
            this.cachedRoomName = PhotonNetwork.CurrentRoom.Name;
        }

        private string cachedRoomName;


        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            //Debug.Log(propertiesThatChanged.ToStringFull());
            //Hashtable props = propertiesThatChanged[(byte)250] as Hashtable;
            //if (props != null) Debug.Log("key 250: "+props.ToStringFull());
        }
        public override void OnPlayerPropertiesUpdate(Player target, Hashtable propertiesThatChanged)
        {
            Debug.LogWarning("Player "+target.ActorNumber+" props: "+propertiesThatChanged.ToStringFull());
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            this.cachedRoomName = "";
            Debug.Log("OnJoinRoomFailed(). "+ returnCode + " msg: "+message);
        }
    }


#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(ConnectAndRejoinOnLeave), true)]
public class ConnectAndRejoinOnLeaveInspector : Editor
{
    public override void OnInspectorGUI()
    {
        this.DrawDefaultInspector(); // Draw the normal inspector

        if (Application.isPlaying)
        {
            if (!PhotonNetwork.IsConnected)
            {
                if (GUILayout.Button("Connect"))
                {
                    // If the user clicks the button, invoke the method immediately.
                    // There are many ways to do this but I chose to use Invoke which only works in Play Mode.
                    ((ConnectAndRejoinOnLeave)this.target).Invoke("ConnectNow", 0f);
                }
            }
            else
            {
                if (GUILayout.Button("Leave"))
                {
                    // If the user clicks the button, invoke the method immediately.
                    // There are many ways to do this but I chose to use Invoke which only works in Play Mode.
                    ((ConnectAndRejoinOnLeave)this.target).Invoke("LeaveNow", 0f);
                }
             }
        }
    }
}
#endif
}
