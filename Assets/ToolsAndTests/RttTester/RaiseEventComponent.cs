#define GRAPHER

using System;
using System.Diagnostics;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;


public class Timings
{
    public int Count;
    public long TravelTimeTotal;
    public long RttTotal;

    public long TravelTimeAverage
    {
        get { return this.Count == 0 ? 0 : this.TravelTimeTotal / this.Count; }
    }

    public long RttAverage
    {
        get { return this.Count == 0 ? 0 : this.RttTotal / this.Count; }
    }

    public override string ToString()
    {
        return string.Format("{0:000} vs {1:000}", this.TravelTimeAverage, this.RttAverage);
    }
}


public class RaiseEventComponent : MonoBehaviourPunCallbacks
{
    private Stopwatch sw = new Stopwatch();
    private RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };

    public bool CallSendoutgoing = true;

    public float SecondsBetweenToggle = 0.0f;
    private bool Toggling;



    public MiniGraphControl OpRttGraphControl;
    public Text Output;


    private Timings PunSend = new Timings();
    private Timings DirectSend = new Timings();
    private byte evCode = 0;

    public bool UpdateUi;
    public bool EnableServerTracing;

    private bool DoToggle
    {
        get { return this.SecondsBetweenToggle >= 0.001f; }
    }

    
    private void Start()
    {
        Application.targetFrameRate = 300;
        QualitySettings.vSyncCount = 0;

        this.sw.Start();
        PhotonNetwork.NetworkingClient.EventReceived += this.OnEvent;
        PhotonNetwork.NetworkingClient.LoadBalancingPeer.EnableServerTracing = this.EnableServerTracing; // this is not available in libraries older than May 2017
        this.InvokeRepeating("RaiseEvents", 1, 0.5f);


        if (this.OpRttGraphControl != null)
        {
            this.OpRttGraphControl.SetName("RTT RaiseEvent-to-Event");
            this.OpRttGraphControl.LowIsGreen = true;
            this.OpRttGraphControl.IgnoreNullForAverage = true;
            this.OpRttGraphControl.IgnoreNullForMin = true;
        }
    }


    public override void OnConnectedToMaster()
    {
        this.Reset();
    }

    // may be called by UI Button
    public void Reset()
    {
        this.evCode++;
        this.PunSend = new Timings();
        this.DirectSend = new Timings();
        this.sw.Reset();
        this.sw.Start();
    }


    private void OnEvent(EventData ev)
    {
        if (ev.Code != this.evCode)
        {
            return;
        }

        int sentTime = (this.SendTimeOnly) ? (int)ev.CustomData : 0;
        long diff = this.sw.ElapsedMilliseconds - sentTime;

        Timings temp = this.CallSendoutgoing ? this.DirectSend : this.PunSend;
        temp.TravelTimeTotal += diff;
        temp.RttTotal += PhotonNetwork.GetPing();
        temp.Count++;

        #if GRAPHER
        // Grapher is a free package from the asset store and offers in-editor graphs for analytics
        // int diffToReceive = SupportClass.GetTickCount() - PhotonNetwork.NetworkingClient.LoadBalancingPeer.TimestampOfLastSocketReceive;

        //Grapher.Log((int)diff, "travel time direct", Color.green);
        //Grapher.Log((int)PhotonNetwork.NetworkingClient.LoadBalancingPeer.RoundTripTime, "rtt direct", Color.blue);
        ////Grapher.Log((int)PhotonNetwork.NetworkingClient.RoundTripTimeCurrent, "rtt last", Color.cyan);  // this is not available in libraries older than May 2017 (comment it out)
        //Grapher.Log((int)this.DirectSend.TravelTimeAverage, "TT avg direct", Color.yellow);
        //Grapher.Log((int)this.DirectSend.RttAverage, "RTT avg direct", Color.red);
        //Grapher.Log((int)diffToReceive, "diff to receive", Color.magenta);
        #endif


        if (this.UpdateUi)
        {
            this.Output.text = string.Format("{0}  by PUN\n{1}  direct send\n{2} ping {5}\n{3:00}:{4:00} running\nserver: {6}", this.PunSend, this.DirectSend, PhotonNetwork.GetPing(), this.sw.Elapsed.Minutes, this.sw.Elapsed.Seconds, PhotonNetwork.CloudRegion, PhotonNetwork.NetworkingClient.CurrentServerAddress);
        }

        if (this.OpRttGraphControl != null)
        {
            this.OpRttGraphControl.AddValue(diff);
        }


        if (this.DoToggle && !this.Toggling)
        {
            this.CancelInvoke("Toggle");
            this.InvokeRepeating("Toggle", 0, this.SecondsBetweenToggle);
            Debug.Log("Toggling started");
        }
        else if (this.Toggling)
        {
            this.CancelInvoke("Toggle");
            this.Toggling = false;
            Debug.Log("Toggling ended");
        }
    }

    private void Toggle()
    {
        this.Toggling = true;
        this.CallSendoutgoing = !this.CallSendoutgoing;
    }

    public bool SendTimeOnly;
    public bool Encrypt;
    private byte[] content;
    private ArraySegment<byte> contentSegment;
    private int contentSize;
    private void RaiseEvents()
    {
        if (!PhotonNetwork.InRoom)
        {
            return;
        }

        SendOptions sendOptions = SendOptions.SendUnreliable;
        sendOptions.Encrypt = this.Encrypt;
        if (this.SendTimeOnly)
        {

            PhotonNetwork.RaiseEvent(this.evCode, (int)this.sw.ElapsedMilliseconds, this.options, sendOptions);
            if (this.CallSendoutgoing)
            {
                PhotonNetwork.SendAllOutgoingCommands();
            }
        }
        else
        {
            if (this.content == null)
            {
                int mtu = PhotonNetwork.NetworkingClient.LoadBalancingPeer.MaximumTransferUnit;
                this.content = new byte[mtu + 10];
                this.contentSize = mtu - 10;
            }

            this.contentSegment = new ArraySegment<byte>(this.content, 0, this.contentSize);
            PhotonNetwork.RaiseEvent(this.evCode, this.contentSegment, this.options, sendOptions);
            this.contentSize = (this.contentSize + 1) % (this.content.Length);

            if (this.CallSendoutgoing)
            {
                PhotonNetwork.SendAllOutgoingCommands();
            }
        }
    }
}