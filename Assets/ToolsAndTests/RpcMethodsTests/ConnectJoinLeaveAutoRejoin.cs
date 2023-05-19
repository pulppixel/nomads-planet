using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;

public class ConnectJoinLeaveAutoRejoin : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private string roomName = "test";
    [SerializeField]
    private byte maxPlayers;
    [SerializeField]
    private int emptyRoomTtl = 300000;
    [SerializeField]
    private int playerTtl = -1;
    [SerializeField]
    private bool publishUserId = true;

    private bool rejoin;

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    
    [ContextMenu("JoinRoom")]
    private void JoinRoom()
    {
        PhotonNetwork.JoinOrCreateRoom(this.roomName, new RoomOptions { EmptyRoomTtl = this.emptyRoomTtl, PlayerTtl = this.playerTtl, MaxPlayers = this.maxPlayers, PublishUserId = this.publishUserId }, TypedLobby.Default);
    }

    [ContextMenu("LeaveRoom")]
    private void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            this.roomName = PhotonNetwork.CurrentRoom.Name;
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        this.rejoin = true;
    }

    public override void OnConnectedToMaster()
    {
        if (this.rejoin)
        {
            PhotonNetwork.RejoinRoom(this.roomName);
            this.rejoin = false;
        }
        else
        {
            this.JoinRoom();
        }
    }
}
