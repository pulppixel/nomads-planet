using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class InRoomCallbacksTimingTest : MonoBehaviourPunCallbacks, IOnEventCallback
{
    private string roomName = Guid.NewGuid().ToString();
    private Hashtable hash;

	public void Start()
    {
        int random = Random.Range(0, 100);
        hash = new Hashtable(1);
        hash.Add("k", random);
        this.Connect();
	}

    private void Connect()
    {
        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.UserId = Guid.NewGuid().ToString();
        PhotonNetwork.ConnectUsingSettings();
    }
    
	public override void OnConnectedToMaster()
	{
		Debug.LogFormat("OnConnectedToMaster LocalPlayer={0}", PhotonNetwork.LocalPlayer.UserId);
        //PhotonNetwork.LocalPlayer.NickName = name;
        
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.EmptyRoomTtl = 300000;
        roomOptions.PlayerTtl = -1;
        roomOptions.BroadcastPropsChangeToAll = true;
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
	}
    
	public override void OnPlayerPropertiesUpdate(Player photonPlayer, Hashtable changedProps)
	{
		Debug.LogFormat("OnPlayerPropertiesUpdate. Player={0} ChangedProps={1} InRoom={2}", photonPlayer.ToStringFull(), SupportClass.DictionaryToString(changedProps), PhotonNetwork.InRoom);
    }

	public override void OnJoinedRoom()
	{
        Debug.LogFormat("OnJoinedRoom LocalPlayer={0}", PhotonNetwork.LocalPlayer.ToStringFull());
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }
	}

    public override void OnRoomPropertiesUpdate(Hashtable changedProps)
    {
        Debug.LogFormat("OnRoomPropertiesUpdate properties={0} InRoom={1}", SupportClass.DictionaryToString(changedProps), PhotonNetwork.InRoom);
        if (changedProps.ContainsKey("k") && PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (cause != DisconnectCause.DisconnectByClientLogic)
        {
            this.Connect();
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == EventCode.PropertiesChanged || photonEvent.Code == EventCode.Join)
        {
            Debug.Log(photonEvent.ToStringFull());
        }
    }
}