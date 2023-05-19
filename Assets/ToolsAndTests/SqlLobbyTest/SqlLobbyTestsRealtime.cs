using Photon.Realtime;
using System.Collections.Generic;

public class RandomMatchmakingExampleRealtime : IMatchmakingCallbacks, ILobbyCallbacks
{
    private TypedLobby sqlLobby = new TypedLobby("myLobby", LobbyType.SqlLobby);
    private LoadBalancingClient loadBalancingClient;

    private void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { "C0", 400 }, { "C3", "Map3" } };
        roomOptions.CustomRoomPropertiesForLobby = new[] { "C0", "C3" }; // this makes "C0" and "C3" available in the lobby
        EnterRoomParams enterRoomParams = new EnterRoomParams();
        enterRoomParams.RoomOptions = roomOptions;
        enterRoomParams.Lobby = this.sqlLobby;
        this.loadBalancingClient.OpJoinOrCreateRoom(enterRoomParams);
    }

    private void JoinRandomRoom()
    {
        string sqlLobbyFilter = "C0 BETWEEN 345 AND 475 AND C3 = 'Map2'";        
        //string sqlLobbyFilter = "C0 > 345 AND C0 < 475 AND (C3 = 'Map2' OR C3 = \"Map3\")";
        //string sqlLobbyFilter = "C0 => 345 AND C0 <= 475 AND C3 IN ('Map1', 'Map2', 'Map3')";
        OpJoinRandomRoomParams opJoinRandomRoomParams = new OpJoinRandomRoomParams();
        opJoinRandomRoomParams.SqlLobbyFilter = sqlLobbyFilter;
        this.loadBalancingClient.OpJoinRandomRoom(opJoinRandomRoomParams);
    }

    private void GetCustomRoomList()
    {
    }

    #region IMatchmakingCallbacks
    
    void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
    {
        this.CreateRoom();
    }

    void IMatchmakingCallbacks.OnJoinedRoom()
    {
        // joined a room successfully
    }

    // rest of the class

    void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
    {
    }

    void IMatchmakingCallbacks.OnCreatedRoom()
    {
    }

    void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
    {
    }

    void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
    {
    }

    void IMatchmakingCallbacks.OnLeftRoom()
    {
    }

    #endregion

    #region ILobbyCallbacks

    void ILobbyCallbacks.OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // here you get the response, empty list if no rooms found
    }

    void ILobbyCallbacks.OnJoinedLobby()
    {
    }

    void ILobbyCallbacks.OnLeftLobby()
    {
    }

    

    void ILobbyCallbacks.OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
    }

    #endregion
}