using UnityEngine;
using System.Collections;
using Photon.Realtime;
using Photon.Pun;

public class CallbackWatcher : MonoBehaviour, IConnectionCallbacks , IMatchmakingCallbacks , IInRoomCallbacks, ILobbyCallbacks {


	public virtual void OnEnable()
	{
		PhotonNetwork.AddCallbackTarget(this);
	}

	public virtual void OnDisable()
	{
		PhotonNetwork.RemoveCallbackTarget(this);
	}

	void LogCallback(string content)
	{
		Debug.Log ("CallbackWatcher: " + content);
	}

	#region IConnectionCallbacks implementation

	public void OnConnected ()
	{
		LogCallback ("OnConnected");
	}

	public void OnConnectedToMaster ()
	{
		LogCallback ("OnConnectedToMaster");
	}

	public void OnDisconnected (DisconnectCause cause)
	{
		LogCallback ("OnDisconnected");
	}

	public void OnRegionListReceived (RegionHandler regionHandler)
	{
		LogCallback ("OnRegionListReceived "+regionHandler.SummaryToCache);
	}

	public void OnCustomAuthenticationResponse (System.Collections.Generic.Dictionary<string, object> data)
	{
		LogCallback ("OnCustomAuthenticationResponse");
	}

	public void OnCustomAuthenticationFailed (string debugMessage)
	{
		LogCallback ("OnCustomAuthenticationFailed");
	}

	#endregion

	#region ILobbyCallbacks implementation

	public void OnJoinedLobby ()
	{
		LogCallback ("OnJoinedLobby");
	}

	public void OnLeftLobby ()
	{
		LogCallback ("OnLeftLobby");
	}

	public void OnRoomListUpdate (System.Collections.Generic.List<RoomInfo> roomList)
	{
		LogCallback ("OnRoomListUpdate");
	}

	public void OnLobbyStatisticsUpdate (System.Collections.Generic.List<TypedLobbyInfo> lobbyStatistics)
	{
		LogCallback ("OnLobbyStatisticsUpdate");
	}

	#endregion

	#region IMatchmakingCallbacks implementation

	public void OnFriendListUpdate (System.Collections.Generic.List<FriendInfo> friendList)
	{
		LogCallback ("OnFriendListUpdate");
	}

	public void OnCreatedRoom ()
	{
		LogCallback ("OnCreatedRoom");
	}

	public void OnCreateRoomFailed (short returnCode, string message)
	{
		LogCallback ("OnCreateRoomFailed");
	}

	public void OnJoinedRoom ()
	{
		LogCallback ("OnJoinedRoom");
	}

	public void OnJoinRoomFailed (short returnCode, string message)
	{
		LogCallback ("OnJoinRoomFailed");
	}

	public void OnJoinRandomFailed (short returnCode, string message)
	{
		LogCallback ("OnJoinRandomFailed");
	}

	public void OnLeftRoom ()
	{
		LogCallback ("OnLeftRoom");
	}

	#endregion

	#region IInRoomCallbacks implementation

	public void OnPlayerEnteredRoom (Player newPlayer)
	{
		LogCallback ("OnPlayerEnteredRoom");
	}

	public void OnPlayerLeftRoom (Player otherPlayer)
	{
		LogCallback ("OnPlayerLeftRoom");
	}

	public void OnRoomPropertiesUpdate (ExitGames.Client.Photon.Hashtable propertiesThatChanged)
	{
		LogCallback ("OnRoomPropertiesUpdate");
	}

	public void OnPlayerPropertiesUpdate (Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
	{
		LogCallback ("OnPlayerPropertiesUpdate");
	}

	public void OnMasterClientSwitched (Player newMasterClient)
	{
		LogCallback ("OnMasterClientSwitched");
	}

	#endregion
}
