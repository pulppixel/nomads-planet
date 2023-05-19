using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SqlLobbyTests : MonoBehaviourPunCallbacks
{
    private TypedLobby sqlLobby = new TypedLobby("myLobby", LobbyType.SqlLobby);
    [SerializeField] private int c0 = 400;
    [SerializeField] private string c1 = "c1";
    [SerializeField] private string c3 = "Map3";
    [SerializeField] private string sqlLobbyFilter = "C0 BETWEEN 345 AND 475 AND C3 = 'Map2'";

    [ContextMenu("CreateRoom")]
    private void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
            {{"C0", this.c0}, {"c1", this.c1}, {"C3", this.c3}};
        roomOptions.CustomRoomPropertiesForLobby =
            new[] {"C0", "c1", "C3"}; // this makes "C0" and "C3" available in the lobby
        PhotonNetwork.CreateRoom(null, roomOptions, this.sqlLobby);
    }

    [ContextMenu("JoinRandomRoom")]
    private void JoinRandomRoom()
    {
        //string sqlLobbyFilter = "C0 > 345 AND C0 < 475 AND (C3 = 'Map2' OR C3 = \"Map3\")";
        //string sqlLobbyFilter = "C0 => 345 AND C0 <= 475 AND C3 IN ('Map1', 'Map2', 'Map3')";
        PhotonNetwork.JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, this.sqlLobby, this.sqlLobbyFilter);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogFormat("OnJoinRandomFailed {0} {1}", returnCode, message);
    }

    public override void OnJoinedRoom()
    {
        // joined a room successfully
    }

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        this.JoinRandomRoom();
    }

    [ContextMenu("GetCustomRoomList")]
    private void GetCustomRoomList()
    {
        PhotonNetwork.GetCustomRoomList(this.sqlLobby, this.sqlLobbyFilter);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.LogFormat("Received list of {0} rooms", roomList.Count);
        for (int i = 0; i < roomList.Count; i++)
        {
            Debug.Log(roomList[i].ToStringFull());
        }
    }
}