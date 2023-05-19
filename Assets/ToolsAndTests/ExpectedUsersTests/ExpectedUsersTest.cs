using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ExpectedUsersTest : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private byte maxPlayers;
    [SerializeField]
    private string roomName = "expectedUsersTests";
    [SerializeField]
    private bool publishUserId = true;
    [SerializeField]
    private string[] expectedUsersOnRandomJoin;
    [SerializeField]
    private string[] expectedUsersOnCreate;
    [SerializeField]
    private string[] expectedUsersOnJoin;
    [SerializeField]
    private string[] expectedUsersToSetInRoom;
    [SerializeField]
    private string[] currentExpectedUsersInRoom;
    [SerializeField]
    private string[] currentJoinedUsers;
    
    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    //private void OnValidate()
    //{
    //    if (PhotonNetwork.InRoom)
    //    {
    //        if (this.maxPlayers != PhotonNetwork.CurrentRoom.MaxPlayers)
    //        {
    //            PhotonNetwork.CurrentRoom.MaxPlayers = this.maxPlayers;
    //        }
    //    }
    //}

    public override void OnJoinedRoom()
    {
        this.maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
        this.roomName = PhotonNetwork.CurrentRoom.Name;
        this.currentExpectedUsersInRoom = PhotonNetwork.CurrentRoom.ExpectedUsers;
        this.currentJoinedUsers = new string[PhotonNetwork.CurrentRoom.Players.Count];
        this.SetJoinedUsers();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {        
        this.SetJoinedUsers();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        this.SetJoinedUsers();
    }

    private void SetJoinedUsers()
    {
        int i = 0;
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            this.currentJoinedUsers[i] = player.UserId;
            i++;
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(GamePropertyKey.ExpectedUsers))
        {
            this.currentExpectedUsersInRoom = PhotonNetwork.CurrentRoom.ExpectedUsers;
        }
        if (propertiesThatChanged.ContainsKey(GamePropertyKey.MaxPlayers))
        {
            this.maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
        }
    }

    public override void OnLeftRoom()
    {
        this.currentExpectedUsersInRoom = null;
        this.currentJoinedUsers = null;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        this.CreateRoomWithExpectedUsers();
    }

    [ContextMenu("JoinRandomRoomWithExpectedUsers")]
    private void JoinRandomRoomWithExpectedUsers()
    {
        if (!PhotonNetwork.JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, null, null, this.expectedUsersOnRandomJoin))
        {
            Debug.LogError("JoinRandomRoom returned false!");
        }
    }

    [ContextMenu("CreateRoomWithExpectedUsers")]
    private void CreateRoomWithExpectedUsers()
    {
        if (!PhotonNetwork.CreateRoom(null, roomOptions: new RoomOptions { PublishUserId = this.publishUserId}, expectedUsers: this.expectedUsersOnCreate))
        {
            Debug.LogError("CreateRoom returned false!");
        }
    }

    [ContextMenu("JoinRoomWithExpectedUsers")]
    private void JoinRoomWithExpectedUsers()
    {
        if (!PhotonNetwork.JoinRoom(this.roomName, this.expectedUsersOnJoin))
        {
            Debug.LogError("JoinRoom returned false!");
        }
    }

    [ContextMenu("SetExpectedUsers")]
    private void SetExpectedUsers()
    {
        if (!PhotonNetwork.InRoom)
        {
            Debug.LogError("Not joined to a room!");
        }
        if (!PhotonNetwork.CurrentRoom.SetExpectedUsers(this.expectedUsersToSetInRoom))
        {
            Debug.LogError("SetExpectedUsers returned false!");
        }
    }
}
