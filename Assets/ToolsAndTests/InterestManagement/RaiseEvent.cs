using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Realtime.Demo;
using UnityEngine;

public class RaiseEvent : MonoBehaviour,IMatchmakingCallbacks, IOnEventCallback {
    
    [Serializable]
    public class InRoomOptions
    {
        public bool ReuseEvent;
        public bool UseByteArraySlice;

        public float EventInterval = 0.1f;
        public int EventBytes = 100;
        public bool SendToSelf = true;

        public bool SendEvent = false;
    }
    
        
    public InRoomOptions InRoomSettings;

    private LoadBalancingClient lbc;

    public int SentEvents = 0;

    void Start()
    {
        this.StartCoroutine(this.StartCoroutine());
    }

	IEnumerator StartCoroutine()
    {
        yield return null;
        this.lbc = this.GetComponent<ConnectInterestManagement>().Client;

        while (this.lbc == null)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            this.lbc = this.GetComponent<ConnectInterestManagement>().Client;
        }

        this.lbc.AddCallbackTarget(this);
    }

    void OnDisable()
    {
        this.StopCoroutine(this.EventSenderCoroutine());

        if (this.lbc != null)
        {
            this.lbc.RemoveCallbackTarget(this);
        }
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

    public void OnJoinedRoom()
    {
        StartCoroutine(this.EventSenderCoroutine());
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
    }

    public void OnLeftRoom()
    {
        this.StopCoroutine(this.EventSenderCoroutine());
    }

    
        
    public IEnumerator EventSenderCoroutine()
    {
        object eventContent = null;
        while (true)
        {
            if (this.InRoomSettings.EventInterval < 0.001f)
            {
                // send nothing
                yield return new WaitForSeconds(1);
                continue;
            }


            if (this.InRoomSettings.SendEvent)
            {
                RaiseEventOptions options = RaiseEventOptions.Default;
                if (this.InRoomSettings.SendToSelf)
                {
                    options = new RaiseEventOptions() { Receivers = ReceiverGroup.All };
                }

                this.lbc.OpRaiseEvent(199, eventContent, options, SendOptions.SendUnreliable);
                this.SentEvents++;
            }

            yield return new WaitForSeconds(this.InRoomSettings.EventInterval);
        }
    }

    

    public void OnEvent(EventData photonEvent)
    {
        Debug.Log("event!");
    }
}
