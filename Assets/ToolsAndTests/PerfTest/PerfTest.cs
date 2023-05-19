using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PerfTest : MonoBehaviour
{
    public string AppId;
    public string AppVersion;
    public string RegionCode;
    public bool Crc;
    public bool AlternativePorts;
    public bool ReuseEvent;
    public byte MaxPlayers = 5;
    public int Mtu = 1200;

    [HideInInspector]
    public int ClientCount;
    [HideInInspector]
    public int ClientCountInRoom;

    private readonly List<LoadBalancingClient> clients = new List<LoadBalancingClient>();
    
    public float EventInterval = 0.2f;
    public int EventSize = 0;
    public bool ReliableEvent = false;

    private byte[] eventBytes;
    private float lastEventTime;

    private ClientStatTracker stats = new ClientStatTracker();


    public uint MonitoredClient;
    private Canvas canvas;
    private MiniGraphControl sentReliableGraph;
    private MiniGraphControl trafficGraph;
    private MiniGraphControl pingGraph;


    public Text UiText;
    public int PropKilobytes = 30;


    public enum SocketType { Default, Poll, Async, Native }
    public SocketType SocketImplementation = SocketType.Default;

    private Type nativeSocketType;

    // Use this for initialization
    void Start()
    {
        this.canvas = GameObject.FindObjectOfType<Canvas>();
        this.trafficGraph = MiniGraphControl.Create("bytes/s", this.canvas.gameObject);
        this.trafficGraph.queueLength = 120;
        this.trafficGraph.SetGraphType(MiniGraphControl.GraphValueType.ValueSum);
        this.trafficGraph.LowIsGreen = true;
        this.trafficGraph.SetUpdateRate(1);
        this.sentReliableGraph = MiniGraphControl.Create("sent reliable", this.canvas.gameObject);
        this.sentReliableGraph.queueLength = 120;
        this.sentReliableGraph.SetGraphType(MiniGraphControl.GraphValueType.ValueMax);
        this.sentReliableGraph.LowIsGreen = true;
        this.sentReliableGraph.SetUpdateRate(1);
        this.pingGraph = MiniGraphControl.Create("ping", this.canvas.gameObject);
        this.pingGraph.queueLength = 120;
        this.pingGraph.SetGraphType(MiniGraphControl.GraphValueType.ValueMax);
        this.pingGraph.LowIsGreen = true;
        this.pingGraph.SetUpdateRate(1);
        //this.trafficGraph.queueLength = 60;

        if (this.UiText != null)
        {
            this.StartCoroutine(this.UpdateUi());
        }

        // try loading the native socket class (which may not exist in some projects)
        this.nativeSocketType = Type.GetType("ExitGames.Client.Photon.SocketUdpNativeSrc, Assembly-CSharp-firstpass", false);
        if (this.nativeSocketType == null)
        {
            this.nativeSocketType = Type.GetType("ExitGames.Client.Photon.SocketUdpNativeSrc, Assembly-CSharp", false);
        }
    }


    IEnumerator UpdateUi()
    {
        while (true)
        {
            int resentCommands = 0;
            int longestSendCall = 0;
            if (this.clients != null && this.clients.Count > 0)
            {
                LoadBalancingClient client = this.clients[(int)this.MonitoredClient];
                resentCommands = client.LoadBalancingPeer.ResentReliableCommands;
                //longestSendCall = client.LoadBalancingPeer.LongestSentCall;
            }

            this.UiText.text = string.Format("In Room: {1}/{0}\nTime: {3:##.00}\nMonitoring: {2}\nResends: {4}\nMTU: {5}\nEventSize: {6}\nSocketType: {7}\nLongestSend: {8}\nRegion: {9}", this.ClientCount, this.ClientCountInRoom, this.MonitoredClient, Time.realtimeSinceStartup, resentCommands, this.Mtu, this.EventSize, this.SocketImplementation, longestSendCall, this.RegionCode);
            yield return new WaitForSeconds(1.0f);
        }
    }


    void Update ()
    {
        if (Input.GetKeyUp(KeyCode.KeypadPlus))
        {
            int count = 1;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                count = 10;
            }
            this.AddClient(count);
            this.MonitoredClient = (uint)this.clients.Count - 1;
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            this.trafficGraph.Reset();
            this.sentReliableGraph.Reset();
            this.pingGraph.Reset();
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            this.MonitoredClient--;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            this.MonitoredClient++;
        }
        if (Input.GetKeyUp(KeyCode.M))
        {
            this.Mtu = this.Mtu == 534 ? 1200 : 534;
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            this.EventSize = this.EventSize == 330 ? 3300 : 330;
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            StartCoroutine("FillProperties");
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            switch (this.SocketImplementation)
            {
                case SocketType.Default:
                    SocketImplementation = SocketType.Poll;
                    break;
                case SocketType.Poll:
                    SocketImplementation = SocketType.Async;
                    break;
                case SocketType.Async:
                    SocketImplementation = SocketType.Native;
                    break;
                case SocketType.Native:
                    SocketImplementation = SocketType.Default;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        this.ClientCount = this.clients.Count;  // this way, the inspector is being updated

        bool sendEvents = Time.time - this.lastEventTime > this.EventInterval;

        if (sendEvents)
        {
            this.lastEventTime = Time.time;

            if (this.eventBytes == null || this.eventBytes.Length != this.EventSize)
            {
                this.eventBytes = new byte[this.EventSize];
            }


            byte evSendCode = this.ReliableEvent ? (byte)1 : (byte)0;
            SendOptions evSendOptions = this.ReliableEvent ? SendOptions.SendReliable: SendOptions.SendUnreliable;
            for (int i = 0; i < this.clients.Count; i++)
            {
                LoadBalancingClient client = this.clients[i];
                if (client.InRoom)
                {
                    client.OpRaiseEvent(evSendCode, this.eventBytes, RaiseEventOptions.Default, evSendOptions);
                }
            }
        }

        this.ClientCountInRoom = 0;
        for (int i = 0; i < this.clients.Count; i++)
        {
            LoadBalancingClient client = this.clients[i];
            client.Service();
            if (client.InRoom)
            {
                this.ClientCountInRoom++;
            }
        }


        // feed the graphs
        if (this.clients.Count > 0 && this.MonitoredClient >= 0)
        {
            if (this.MonitoredClient >= this.clients.Count)
            {
                this.MonitoredClient = (uint)this.clients.Count - 1;
            }

            LoadBalancingClient client = this.clients[(int)this.MonitoredClient];
            this.stats.Client = client;
            this.stats.Update();
            this.trafficGraph.AddValue(this.stats.Delta.BytesIn+this.stats.Delta.BytesOut);
            this.sentReliableGraph.AddValue(client.LoadBalancingPeer.SentReliableCommandsCount);
            this.pingGraph.AddValue(client.LoadBalancingPeer.RoundTripTime);
            
        } 
    }

    public IEnumerator FillProperties()
    {
        yield return new WaitForSeconds(0.2f);
        Hashtable props = new Hashtable() { { "load", new byte[1024 * this.PropKilobytes] } };
        this.clients[(int)this.MonitoredClient].CurrentRoom.SetCustomProperties(props);
    }

    public void AddClient(int count = 1)
    {
        for (int i = count; i > 0; i--)
        {
            LoadBalancingClient c = new LoadBalancingClient();
            c.AppId = this.AppId;
            c.AppVersion = this.AppVersion;
            c.LoadBalancingPeer.ChannelCount = 1;
            c.LoadBalancingPeer.CrcEnabled = this.Crc;
            c.LoadBalancingPeer.ReuseEventInstance = this.ReuseEvent;
            c.UseAlternativeUdpPorts = this.AlternativePorts;
            c.LoadBalancingPeer.MaximumTransferUnit = this.Mtu;

            
            switch (this.SocketImplementation)
            {
                case SocketType.Default:
                    c.LoadBalancingPeer.SocketImplementationConfig = null;
                    break;
                case SocketType.Poll:
                    //c.LoadBalancingPeer.SocketImplementationConfig[ConnectionProtocol.Udp] = typeof(SocketUdpPoll);
                    break;
                case SocketType.Async:
                    c.LoadBalancingPeer.SocketImplementationConfig[ConnectionProtocol.Udp] = typeof(SocketUdpAsync);
                    break;
                case SocketType.Native:
                    
                    if (nativeSocketType != null)
                    {
                        c.LoadBalancingPeer.SocketImplementationConfig[ConnectionProtocol.Udp] = nativeSocketType;
                    }
                    else
                    {
                        Debug.LogWarning("Native socket implementation not found. Using defaults.");
                        c.LoadBalancingPeer.SocketImplementationConfig = null;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            c.ConnectToRegionMaster(this.RegionCode);
            

            ClientWrapper wrapper = new ClientWrapper(c);
            wrapper.MaxPlayersPerRoom = this.MaxPlayers;
            c.AddCallbackTarget(wrapper);

            this.clients.Add(c);
        }
    }

    /// <summary>Called by Unity when the application is closed. Disconnects.</summary>
    protected virtual void OnApplicationQuit()
    {
        SupportClass.StopAllBackgroundCalls();

        for (int i = 0; i < this.clients.Count; i++)
        {
            LoadBalancingClient client = this.clients[i];
            if (client != null && client.IsConnected)
            {
                client.Disconnect();
                client.LoadBalancingPeer.StopThread();
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PerfTest))]
public class PerfTestInspector : Editor
{

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("lala");
        this.DrawDefaultInspector();
        if (Application.isPlaying)
        {
            PerfTest script = (PerfTest)target;
            
            EditorGUILayout.LabelField("Runtime");
            EditorGUILayout.LabelField("Clients", script.ClientCount.ToString());
            EditorGUILayout.LabelField("In Room", script.ClientCountInRoom.ToString());

        }
    }
}
#endif

public class ValuesAtTime
{
    public int Time;
    public long BytesIn;
    public long BytesOut;
    public int Packages;
    

    public ValuesAtTime(LoadBalancingPeer peer = null)
    {
        this.Time = (int)(UnityEngine.Time.realtimeSinceStartup * 1000.0f);
        if (peer != null)
        {
            this.BytesIn = peer.BytesIn;
            this.BytesOut = peer.BytesOut;
        }
    }

    public static ValuesAtTime operator -(ValuesAtTime a, ValuesAtTime b)
    {
        ValuesAtTime result = new ValuesAtTime();
        result.BytesIn = a.BytesIn - b.BytesIn;
        result.BytesOut = a.BytesOut - b.BytesOut;
        result.Time = a.Time - b.Time;
        result.Packages = a.Packages - b.Packages;
        return result;
    }
    
    public override string ToString()
    {
        return string.Format("{0}ms {1}b in {2}b out", this.Time, this.BytesIn, this.BytesOut);
    }
}

public class ClientStatTracker
{
    private ValuesAtTime last;

    public ValuesAtTime Delta;

    private string peerId;

    private LoadBalancingClient client;
    public LoadBalancingClient Client
    {
        get { return this.client; }
        set
        {
            if (value == null || value.Equals(this.client))
            {
                return;
            }

            this.client = value;
            this.Reset();
        }
    }

    public void Reset()
    {
        this.last = new ValuesAtTime(this.client.LoadBalancingPeer);
        this.peerId = this.client.LoadBalancingPeer.PeerID;
    }

    public void Update()
    {
        if (!this.peerId.Equals(this.client.LoadBalancingPeer.PeerID))
        {
            // peerId changed, so there was a reconnect
            this.Reset();
            return;
        }

        ValuesAtTime prev = this.last;
        this.last = new ValuesAtTime(this.client.LoadBalancingPeer);

        this.Delta = this.last - prev;
    }

    public override string ToString()
    {
        if (this.Delta == null)
        {
            return "No delta yet.";
        }
        return this.Delta.ToString();
    }
}



public class ClientWrapper : IMatchmakingCallbacks, IConnectionCallbacks, IOnEventCallback
{
    private bool debug = false;
    private LoadBalancingClient client;

    public byte MaxPlayersPerRoom = 5;

    public ClientWrapper(LoadBalancingClient client)
    {
        this.client = client;
    }

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
    }

    public void OnCreatedRoom()
    {
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
        if (debug) Debug.Log("OnCreateRoomFailed");
    }

    public void OnJoinedRoom()
    {
        if (debug) Debug.Log("JoinedRoom");
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        if (debug) Debug.Log("OnJoinRandomFailed()");
        this.client.OpCreateRoom(new EnterRoomParams() { RoomOptions = new RoomOptions() { MaxPlayers = this.MaxPlayersPerRoom } });
    }

    public void OnLeftRoom()
    {
    }

    public void OnConnected()
    {
    }

    public void OnConnectedToMaster()
    {
        if (debug) Debug.Log("OnConnectedToMaster()");
        this.client.OpJoinRandomRoom();
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected Cause: "+cause+". PeerID "+this.client.LoadBalancingPeer.PeerID + " Duration: "+(this.client.LoadBalancingPeer.ConnectionTime)+"("+(this.client.LoadBalancingPeer.ConnectionTime/1000/60)+"min+) LastSendAckTime: "+this.client.LoadBalancingPeer.LastSendAckTime+" LastSendOutgoingTime: "+this.client.LoadBalancingPeer.LastSendOutgoingTime+" Server: "+this.client.CurrentServerAddress);
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
    }

    public void OnEvent(EventData photonEvent)
    {
        
    }
}
