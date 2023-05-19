#if (NET_4_6 || NET_STANDARD_2_0) && (UNITY_EDITOR || UNITY_STANDALONE)
#define PCAP_SUPPORT
#endif

using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;


/// <summary>
/// Sets up a Traffic Recorder to test. This is a compact sample of how to use Traffic Capturing.
/// </summary>
/// <remarks>
/// A real project should do a few things differently:
///
/// Store the chunks on demand only! E.g. when the connection failed (timeout).
/// This could be done on a webservice or file(s). Make sure you get access to this from any device.
///
/// OnChunkComplete may be used to handle individual memory chunks, versus saving/sending them all
/// at the end of a session.<br/>
/// Alternatively, get all chunks by accessing the TrafficRecorderSrc.CaptureChunkQueue after something happened.
///
/// Use a file name to indicate the server and peerId of the chunks. This makes it easier to identify them.
/// For the pcap files, we strongly suggest this structure:<br/>
/// Photon-Client-[serverIp]-[serverport]-[peerid]-[timestamp].pcap
///
/// You should not convert the capture chunks into pcap format at (game) runtime.
/// Build an executable, which turns the chunk-format into pcap on your developer machine.
/// This can even be done in Unity (headless mode, commandline parameters).
///
/// PacketDotNet and SharpPcap are used to dump captured data into pcap format for Wireshark.
/// These libs require .Net 4.6 or .Net Standard projects.
/// </remarks>
public class TrafficRecorderControl : MonoBehaviour, IConnectionCallbacks
{
    public bool DebugTrafficRecorder; // set in inspector

    private TrafficRecorderSrc trafficRecorder = new TrafficRecorderSrc();

        
    private LoadBalancingClient client;
    public LoadBalancingClient Client
    {
        get { return this.client; }
        set
        {
            if (this.client != null)
            {
                this.client.RemoveCallbackTarget(this);
            }

            this.client = value;

            if (this.client != null)
            {
                this.client.AddCallbackTarget(this);
            }
        }
    }


    void Start()
    {
        if (this.Client == null)
        {
            Debug.LogWarning("TrafficRecorderControl can only work when the Client is assigned (non-null). Disabling self.");
            this.enabled = false;
            return;
        }

        this.trafficRecorder.ChunkSize = 40000; // unrealistically tiny (for testing). should be 500.000.
        this.trafficRecorder.ChunkCountMax = 10;
        this.trafficRecorder.Enabled = this.enabled;
        this.trafficRecorder.Debug = this.DebugTrafficRecorder;
        this.trafficRecorder.OnChunkComplete += this.OnChunkCompleted;

        this.Client.LoadBalancingPeer.TrafficRecorder = this.trafficRecorder;
    }


    void OnApplicationQuit()
    {
        this.Client = null;

        // on application quit, the recording should stop, write a file (if needed) and close it
        // as the engine will call various OnApplicationQuit implementations in any order, this recording may miss a few messages
        // especially the client's "disconnect" message might be missing ...

        #if PCAP_SUPPORT
        if (this.trafficRecorder != null)
        {
            this.trafficRecorder.Enabled = false;
            this.trafficRecorder.CompleteCurrentChunk();
            this.trafficRecorder.CloseFile();
        }
        #endif
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="chunkBuffer"></param>
    /// <param name="recorder"></param>
    private void OnChunkCompleted(StreamBuffer chunkBuffer, TrafficRecorderSrc recorder)
    {
        if (this.DebugTrafficRecorder)
        {
            Debug.Log("Chunk full. Count: " + recorder.ChunkCount);
        }


        //// store this locally or send it somewhere


        //// if you want to turn the chunks to a pcap, use this:
        //List<CapturedMessage> messages = this.trafficRecorder.ChunkToMessageList(chunkBuffer);


        //#if PCAP_SUPPORT
        //if (this.DebugTrafficRecorder)
        //{
        //    Debug.Log("Writing chunk " + recorder.ChunkCount + " to file (append). PeerId: " + messages[0].PeerId.ToString("X4"));
        //}

        //string filename = "server-" + this.Client.LoadBalancingPeer.ServerIpAddress.Replace(':','_') + "-peerId-" + messages[0].PeerId.ToString("X4") + ".pcap";
        //this.trafficRecorder.WriteMessagesToPcap(messages, filename);
        //#endif
    }


    public void OnDisconnected(DisconnectCause cause)
    {
        if (cause != DisconnectCause.DisconnectByClientLogic && this.Client.LoadBalancingPeer.BytesIn > 0)
        {
           Debug.Log("Disconnected?! PeerId: " + this.Client.LoadBalancingPeer.PeerID + " " + this.Client.LoadBalancingPeer.ServerIpAddress);

            #if PCAP_SUPPORT
            foreach (var chunk in this.trafficRecorder.CaptureChunkQueue)
            {
                int temp = chunk.Position;
                chunk.Position = 0;
                var messages = this.trafficRecorder.ChunkToMessageList(chunk);
                chunk.Position = temp;
                if (messages[0].PeerId == 0)
                {
                    //Debug.Log("Found wrong peerId");
                    continue;
                }



                string filename = "server-" + this.Client.LoadBalancingPeer.ServerIpAddress.Replace(':','_') + "-peerId-" + messages[0].PeerId.ToString("X4") + ".pcap";
                this.trafficRecorder.WriteMessagesToPcap(messages, filename);
            }

            this.trafficRecorder.CloseFile();
            #endif
        }
    }



    void Update()
    {
    }

    public void OnConnected()
    {

    }

    public void OnConnectedToMaster()
    {

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
}