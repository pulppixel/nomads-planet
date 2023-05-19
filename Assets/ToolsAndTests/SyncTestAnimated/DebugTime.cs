using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;


public class DebugTime : MonoBehaviour, IMatchmakingCallbacks {

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {



	    if (!PhotonNetwork.InRoom)
	    {
	        return;
	    }

	    if (Math.Round(PhotonNetwork.Time - this.lasttimePun, 4) < 0.0f)
	    {
	        Debug.LogError("PN.time-lasttime is negative");
	    }
        if (lasttimePun != 0) Debug.Log(string.Format("{0} versus {1} - deltatime: {2}",Math.Round(PhotonNetwork.Time-this.lasttimePun, 4), Math.Round(Time.time - this.lasttimeUnity, 4), Time.deltaTime));

	    this.lasttimePun = PhotonNetwork.Time;
	    this.lasttimeUnity = Time.time;
    }

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
    }

    public void OnCreatedRoom()
    {
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
    }

    private double lasttimePun;
    private double lasttimeUnity;

    public void OnJoinedRoom()
    {
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
    }

    public void OnLeftRoom()
    {
    }
}
