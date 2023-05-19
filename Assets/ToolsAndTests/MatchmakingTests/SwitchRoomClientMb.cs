using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class SwitchRoomClientMb : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, IOnEventCallback, IInRoomCallbacks
{
    private LoadBalancingClient lbc;
    public AppSettings Settings;

    public string TargetRoomName = "test";
    public int Iteration = 1;
    public int TargetPlayerCount;


    void Start ()
    {
        this.TargetPlayerCount = FindObjectsOfType<SwitchRoomClientMb>().Length;

        lbc = new LoadBalancingClient();
        this.lbc.AddCallbackTarget(this);
        this.lbc.ConnectUsingSettings(this.Settings);

        StartCoroutine(this.ServiceCoroutine());
    }

    void OnDisable()
    {
        this.lbc.Disconnect();
        this.lbc.LoadBalancingPeer.StopThread();
    }

    private IEnumerator ServiceCoroutine()
    {
        var yielder = new WaitForSecondsRealtime(0.05f);
        
        while (this.isActiveAndEnabled)
        {
            this.lbc.Service();
            yield return yielder;
        }
    }


    void Update () 
    {
		
	}


    /// <inheritdoc />
    public void OnConnected()
    {
        
    }

    /// <inheritdoc />
    public void OnConnectedToMaster()
    {
        this.lbc.OpJoinOrCreateRoom(new EnterRoomParams() { RoomName = string.Format("{0}_{1}", this.TargetRoomName, this.Iteration) });
    }

    /// <inheritdoc />
    public void OnDisconnected(DisconnectCause cause)
    {
        
    }

    /// <inheritdoc />
    public void OnRegionListReceived(RegionHandler regionHandler)
    {
        
    }

    /// <inheritdoc />
    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        
    }

    /// <inheritdoc />
    public void OnCustomAuthenticationFailed(string debugMessage)
    {
        
    }

    /// <inheritdoc />
    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        
    }

    /// <inheritdoc />
    public void OnCreatedRoom()
    {
        
    }

    /// <inheritdoc />
    public void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("OnCreateRoomFailed Error {0}: {1}", returnCode, message);
    }

    /// <inheritdoc />
    public void OnJoinedRoom()
    {
        
    }

    /// <inheritdoc />
    public void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("OnJoinRoomFailed Error {0}: {1}", returnCode, message);
    }

    /// <inheritdoc />
    public void OnJoinRandomFailed(short returnCode, string message)
    {
        
    }

    /// <inheritdoc />
    public void OnLeftRoom()
    {
        
    }

    /// <inheritdoc />
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 1)
        {
            this.Iteration = (int)photonEvent.CustomData;
            this.lbc.OpLeaveRoom(false);
        }
    }

    /// <inheritdoc />
    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (this.lbc.LocalPlayer.IsMasterClient && this.lbc.CurrentRoom.PlayerCount >= TargetPlayerCount)
        {
            Debug.Log("Iteration "+this.Iteration+" done. Sending event to join next room.");

            this.Iteration++;
            this.lbc.OpRaiseEvent(1, this.Iteration, new RaiseEventOptions() {Receivers = ReceiverGroup.All}, SendOptions.SendReliable);
        }
    }

    /// <inheritdoc />
    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        
    }

    /// <inheritdoc />
    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
     
    }

    /// <inheritdoc />
    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        
    }

    /// <inheritdoc />
    public void OnMasterClientSwitched(Player newMasterClient)
    {
        
    }
}
