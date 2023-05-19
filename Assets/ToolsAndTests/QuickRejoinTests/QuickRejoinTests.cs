using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class QuickRejoinTests : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private string prefabName = "test";
    [SerializeField]
    private string roomName = "test";
    [SerializeField]
    private bool cleanupCacheOnLeave = false;
    [SerializeField]
    private int playerTTL = -1;
    [SerializeField]
    private int emptyRoomTTL = 300000;

    private bool rejoining;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
        PhotonNetwork.NetworkingClient.StateChanged += OnStateChanged;
    }

    private void OnStateChanged(ClientState previous, ClientState current)
    {
        Debug.LogFormat("OnStateChanged from {0} to {1}", previous, current);
    }

    private void OnEventReceived(EventData eventData)
    {
        switch (eventData.Code)
        {
            case 202:
                Debug.LogFormat("OnEventReceived: {0}", eventData.ToStringFull());
                break;
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.LogFormat("OnConnectedToMaster");
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions
        {
            PlayerTtl = playerTTL,
            CleanupCacheOnLeave = cleanupCacheOnLeave,
            EmptyRoomTtl = emptyRoomTTL
        }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        if (rejoining)
        {
            rejoining = false;
            return;
        }
        PhotonNetwork.Instantiate(prefabName, Vector3.one, Quaternion.identity);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogFormat("OnJoinRandomFailed {0} {1}", returnCode, message);
        if (rejoining)
        {
            rejoining = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogFormat("OnDisconnected ({0})", cause);
        rejoining = PhotonNetwork.ReconnectAndRejoin();
        Debug.LogFormat("ReconnectAndRejoin returned {0}", rejoining);
    }
    
    [ContextMenu("Disconnect")]
    private void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }
}
