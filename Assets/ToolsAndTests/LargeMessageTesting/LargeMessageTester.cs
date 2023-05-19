using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


public class LargeMessageTester : MonoBehaviourPunCallbacks, IOnEventCallback
{
    private static string BuildInfo = string.Empty;
    
    private bool joinedRoom;
    private bool disconnectByServerLogicOccurred;
    private bool disconnectByClientTimeoutOccurred;

    private float joinedRoomTime;
    private float disconnectedTime;

    private int lastSocketReceive;

    private string disconnectReason;

    [Range(0, 512)]
    public int MessageSizeInKB = 1;

    public bool ApplySocketUdpAsync;
    public bool UseSingleRoom;
    public bool CrcCheck;

    private int SentEvents;
    private int ReceivedEvents;
    private int lastSendBytes;


    public uint EventNumber;

    public int MaxDatagrams;

    private float DeltaSendReceive;
    private float DeltaSendReceiveAvg;
    private float DeltaSendReceiveSum;


    public float FixedSendInterval;
    public bool ReceiveTriggersEvent;
    public float ReceiveTriggersEventDelay = 1.0f;



    private HashSet<uint> ExpectedEvents = new HashSet<uint>();
    private bool updateExpectedEvents;


    private string expectedEventsString;
    private float runtime;


    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            this.ReceivedEvents++;

            byte[] content = photonEvent.CustomData as byte[];
            int evNumber;
            float sentTime;
            bool readOk = this.ReadEvent(content, out evNumber, out sentTime);
            if (!readOk)
            {
                Debug.LogWarning("Could not read event. " + SupportClass.ByteArrayToString(content));
            }
            else
            {
                float deltaTime = Time.realtimeSinceStartup - sentTime;
                this.DeltaSendReceive = deltaTime;
                this.DeltaSendReceiveSum += deltaTime;
                this.DeltaSendReceiveAvg = this.DeltaSendReceiveSum / this.ReceivedEvents;
            }


            if (this.ReceiveTriggersEvent)
            {
                this.Invoke("SendEvent", this.ReceiveTriggersEventDelay);
            }
        }
    }

    public void Awake()
    {
        BuildInfo += "PUN v" + PhotonNetwork.PunVersion + "  ";
        #if NET_4_6
        BuildInfo += "Scripting Runtime Version: .NET 4.x ";
        #elif NET_STANDARD_2_0
        BuildInfo += "Scripting Runtime Version: .NET Standard 2 ";
        #else
        BuildInfo += "Scripting Runtime Version: .NET 3.5 ";
        #endif

        #if ENABLE_MONO
        BuildInfo += "ENABLE_MONO ";
        #endif
        #if ENABLE_IL2CPP
        BuildInfo += "ENABLE_IL2CPP ";
        #endif
        #if ENABLE_DOTNET
        BuildInfo += "ENABLE_DOTNET ";
        #endif


        if (this.ApplySocketUdpAsync)
        {
            Debug.Log("Applying: SocketUdpAsync");
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.SocketImplementationConfig[ConnectionProtocol.Udp] = typeof(SocketUdpAsync);
        }

        PhotonNetwork.CrcCheckEnabled = this.CrcCheck;
        PhotonNetwork.ConnectUsingSettings();
        PhotonHandler.MaxDatagrams = this.MaxDatagrams;
    }


    private byte[] WriteEvent()
    {
        int targetSizeInByte = this.MessageSizeInKB * 1024;
        if (targetSizeInByte <= 0)
        {
            targetSizeInByte = 8;
        }

        byte[] eventContent = new byte[targetSizeInByte];
        int offset = 0;
        Protocol.Serialize((int)this.EventNumber, eventContent, ref offset);
        Protocol.Serialize(Time.realtimeSinceStartup, eventContent, ref offset);
        //Debug.Log("Wrote: "+(uint)this.EventNumber+ " "+SupportClass.ByteArrayToString(eventContent));

        this.ExpectedEvents.Add(this.EventNumber);
        this.updateExpectedEvents = true;

        this.EventNumber = this.EventNumber + 1;
        return eventContent;
    }

    private bool ReadEvent(byte[] eventContent, out int number, out float sentTime)
    {
        number = -1;
        sentTime = 0;
        if (eventContent == null || eventContent.Length < 0)
        {
            return false;
        }


        int offset = 0;
        Protocol.Deserialize(out number, eventContent, ref offset);
        Protocol.Deserialize(out sentTime, eventContent, ref offset);
        //Debug.Log("Read: "+(uint)number + " "+SupportClass.ByteArrayToString(eventContent));

        this.ExpectedEvents.Remove((uint)number);
        this.updateExpectedEvents = true;

        return true;
    }

    private void SendEvent()
    {
        if (!PhotonNetwork.InRoom)
        {
            return;
        }

        byte[] eventContent = this.WriteEvent();
        PhotonNetwork.RaiseEvent((byte)PhotonNetwork.LocalPlayer.ActorNumber, eventContent, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        this.lastSendBytes = PhotonNetwork.NetworkingClient.LoadBalancingPeer.ByteCountLastOperation;
        this.SentEvents++;
        PhotonHandler.SendAsap = true;
    }


    public void ApplySettings()
    {
        this.CancelInvoke("SendEvent");
        if (this.FixedSendInterval > 0)
        {
            this.InvokeRepeating("SendEvent", 0, this.FixedSendInterval);
        }
        else if (this.ReceiveTriggersEvent)
        {
            this.SendEvent();
        }
    }


    public void Update()
    {
        PhotonHandler.MaxDatagrams = this.MaxDatagrams;
    }

    public void OnGUI()
    {
        if (this.updateExpectedEvents)
        {
            this.updateExpectedEvents = false;
            this.expectedEventsString = string.Empty;
            if (this.ExpectedEvents.Count > 0)
            {
                List<uint> sorted = this.ExpectedEvents.ToList();
                sorted.Sort();
                foreach (uint expectedEvent in sorted)
                {
                    this.expectedEventsString += expectedEvent + ", ";
                }
            }
            else
            {
                this.expectedEventsString = "none";
            }
        }

        if (PhotonNetwork.IsConnected)
        {
            this.runtime = (float)Math.Round(Time.realtimeSinceStartup, 2);
            this.lastSocketReceive = SupportClass.GetTickCount() - PhotonNetwork.NetworkingClient.LoadBalancingPeer.TimestampOfLastSocketReceive;
        }


        GUILayout.BeginVertical();
        GUILayout.Label(BuildInfo);


        GUILayout.Space(10);


        GUILayout.Label(string.Format("Connection Status: {0}  {1}  Runtime: {2:0.00}", PhotonNetwork.NetworkingClient.State, PhotonNetwork.InRoom ? " Players: " + PhotonNetwork.CurrentRoom.PlayerCount : "", this.runtime));
        GUILayout.Label(string.Format("Ping: {0,3}   Resends: {1,3}   CrcFails: {3,3}   LastSocketReceive: {2,4}", PhotonNetwork.GetPing(), PhotonNetwork.ResentReliableCommands, this.lastSocketReceive, PhotonNetwork.CrcCheckEnabled ? PhotonNetwork.PacketLossByCrcCheck.ToString():"off"));

        GUILayout.Space(10);


        GUILayout.Label("Message KB: " + this.MessageSizeInKB);
        this.MessageSizeInKB = (int)GUILayout.HorizontalSlider(this.MessageSizeInKB, 0, 512, GUILayout.MaxWidth(300));

        GUILayout.Label("Interval: " + this.FixedSendInterval);
        float newVal = GUILayout.HorizontalSlider(this.FixedSendInterval, 0, 1, GUILayout.MaxWidth(300));
        newVal = (float)Math.Round(newVal, 2);
        if (!Mathf.Approximately(newVal, this.FixedSendInterval))
        {
            this.FixedSendInterval = newVal;
            this.ApplySettings();
        }


        GUILayout.Space(10);


        GUILayout.Label("SendEvent calls (RaiseEvent): " + this.SentEvents);
        GUILayout.Label("Received events: " + this.ReceivedEvents);
        GUILayout.Label("DeltaSendReceive: " + this.DeltaSendReceive.ToString("0.000") + "  Avg: " + this.DeltaSendReceiveAvg.ToString("0.000"));
        GUILayout.Label("Bytes of last send: " + this.lastSendBytes);

        GUILayout.Label("Expected events: " + this.expectedEventsString);


        GUILayout.Space(20);
        GUILayout.Label("<color=green>OnJoinedRoom called " + this.joinedRoomTime.ToString("#.##") + " seconds after application start.</color>");


        if (this.disconnectReason != null)
        {
            GUILayout.Label(this.disconnectReason);
        }

        GUILayout.EndVertical();
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        this.disconnectedTime = Time.time;
        this.disconnectReason = string.Format("<color=orange>OnDisconnected after {0} seconds. Reason: {1}</color>", this.disconnectedTime.ToString("#.##"), cause);

        switch (cause)
        {
            case DisconnectCause.ClientTimeout:
            {
                this.disconnectByClientTimeoutOccurred = true;

                if (this.lastSocketReceive > 1000)
                {
                    this.disconnectReason += "<color=red>Unexpected: this might be a bug with .NET 4.x. (lastSocketReceive > 1000)</color>";
                }
                else
                {
                    this.disconnectReason += "<color=orange>Expected: this might happen due to repeated loss.</color>";
                }

                Debug.LogError(this.disconnectReason);

                break;
            }

            case DisconnectCause.DisconnectByServerLogic:
            {
                this.disconnectByServerLogicOccurred = true;

                break;
            }
        }
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        if (this.UseSingleRoom)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.CreateRoom("TestRoom-" + Guid.NewGuid(), new RoomOptions(), null);
        }
    }

    public override void OnJoinedRoom()
    {
        this.joinedRoom = true;
        this.joinedRoomTime = Time.time;

        this.ApplySettings();
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 }, null);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
    }
}