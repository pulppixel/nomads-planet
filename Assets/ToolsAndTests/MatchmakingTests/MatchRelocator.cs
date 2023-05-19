using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(ConnectionHandler))]
public class MatchRelocator : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, IInRoomCallbacks, IOnEventCallback
{
    public string MatchRegion = "eu";

    public string AppId; // set in inspector
    private LoadBalancingClient lbc;

    private ConnectionHandler ch;
    public Text StateUiText;
    private const string RegionPerfKey = "RegPerf";
    private string regionPerformance;

    public bool RelocateOnRoomFull = true;
    private string targetRoomName;
    private string targetRegion;

    /// <summary>
    /// This event gets called when the match was relocated to another region.
    /// </summary>
    private event Action Relocated;


    public void Start()
    {
        this.CreateClientAndConnect();

        // some script should subscribe for the event Relocated:
        this.Relocated += this.OnRelocated;
    }

    private void OnRelocated()
    {
        Debug.Log("Relocated successfully. Start your game.");
    }

    public void CreateClientAndConnect()
    {
        this.lbc = new LoadBalancingClient();
        this.lbc.AppId = this.AppId;
        this.lbc.AddCallbackTarget(this);
        this.lbc.ConnectToNameServer();

        this.ch = this.gameObject.GetComponent<ConnectionHandler>();
        if (this.ch != null)
        {
            this.ch.Client = this.lbc;
            this.ch.StartFallbackSendAckThread();
        }
    }

    public void Update()
    {
        LoadBalancingClient client = this.lbc;
        if (client != null)
        {
            client.Service();


            string state = client.State.ToString();
            int players = (this.lbc.InRoom) ? this.lbc.CurrentRoom.PlayerCount : 0;
            if (this.StateUiText != null && !this.StateUiText.text.Equals(state))
            {
                this.StateUiText.text = "State: " + state + " region: " + this.lbc.CloudRegion + " players: " + players;
            }
        }
    }

    /// <summary>A callback of the RegionHandler, provided in OnRegionListReceived.</summary>
    /// <param name="regionHandler">The regionHandler wraps up best region and other region relevant info.</param>
    private void OnRegionPingCompleted(RegionHandler regionHandler)
    {
        Debug.Log("OnRegionPingCompleted " + regionHandler.BestRegion);
        Debug.Log("Regions: " + regionHandler.EnabledRegions.ToStringFull());
        Debug.Log("RegionPingSummary: " + regionHandler.SummaryToCache);

        this.CacheRegionPings(regionHandler);

        bool matchRegionAvailable = regionHandler.EnabledRegions.Exists(x => x.Code.Equals(this.MatchRegion));
        Debug.Log("matchRegionAvailable: " + matchRegionAvailable);
        if (matchRegionAvailable)
        {
            this.lbc.ConnectToRegionMaster(this.MatchRegion);
            this.SetRegionPerformanceProperty();
        }
        else
        {
            this.lbc.ConnectToRegionMaster(regionHandler.BestRegion.Code);
        }
    }

    private void CacheRegionPings(RegionHandler regionHandler)
    {
        string[] sb = new string[regionHandler.EnabledRegions.Count];
        for (int i = 0; i < regionHandler.EnabledRegions.Count; i++)
        {
            Region o = regionHandler.EnabledRegions[i];
            sb[i] = (o != null) ? o.ToString(true) : "null";
        }

        this.regionPerformance = string.Join(",", sb);
        Debug.Log("regionPerformance: " + this.regionPerformance);
    }

    private void SetRegionPerformanceProperty()
    {
        Hashtable props = new Hashtable() { { RegionPerfKey, this.regionPerformance } };
        this.lbc.LocalPlayer.SetCustomProperties(props);
    }

    public void RelocateMatch()
    {
        Debug.Log("RelocateMatch");

        if (!string.IsNullOrEmpty(this.targetRoomName))
        {
            // we potentially arrived in the target region and room
            Debug.Log(this.lbc.CloudRegion + " "+ this.lbc.CurrentRoom.Name);
            return;
        }


        // sum each region's ping and find the best for all players
        // TODO: make sure the regions are available in each users list. select one that is.
        List<Region> regionPerf = new List<Region>();
        foreach (Player player in this.lbc.CurrentRoom.Players.Values)
        {
            if (!player.CustomProperties.ContainsKey(RegionPerfKey))
            {
                Debug.LogWarning("RegionPerfKey not found for player: "+player);
                continue;
            }

            string playerPerf = player.CustomProperties[RegionPerfKey] as string;
            string[] regionSplit = playerPerf.Split(new string[] { ",", ":" }, StringSplitOptions.None);

            for (int i = 0; i < regionSplit.Length; i+=2)
            {
                string code = regionSplit[i];
                int ping = Int32.Parse(regionSplit[i + 1]);
                Region found = regionPerf.Find(x => x.Code.Equals(code));
                if (found == null)
                {
                    found = new Region(code, ping);
                    regionPerf.Add(found);
                }
                else
                {
                    found.Ping += ping;
                }
            }

            regionPerf.Sort((a, b) => { return (a.Ping == b.Ping) ? 0 : (a.Ping < b.Ping) ? -1 : 1; });     // could be done at the end and just once
            Debug.Log("regionPerf: "+regionPerf.ToStringFull() + " going to: "+ regionPerf[0].Code);
        }


        if (this.lbc.CloudRegion.StartsWith(regionPerf[0].Code))
        {
            // this region is the best overall. stay here. finish reloaction anyways:
            this.FinishRelocation();
        }
        else
        {
            // when we found the best region for everyone, send the event to relocate
            this.lbc.OpRaiseEvent(199, regionPerf[0].Code, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }
    }


    private void FinishRelocation()
    {
        this.targetRegion = string.Empty;
        this.targetRoomName = string.Empty;
        if (this.Relocated != null)
        {
            this.Relocated();
        }
    }


    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 199)
        {
            this.targetRegion = photonEvent.CustomData as string;
            this.targetRoomName = this.lbc.CurrentRoom.Name;
            Debug.Log("Got 199 / Relocate event: " + this.targetRegion);

            this.lbc.Disconnect();
        }
    }


    public void OnConnected()
    {
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster. Region: " + this.lbc.CloudRegion);

        if (this.targetRoomName != null)
        {
            this.lbc.OpJoinOrCreateRoom(new EnterRoomParams() {RoomName = this.targetRoomName, RoomOptions = new RoomOptions() {IsVisible = false } });
        }
        else
        {
            this.lbc.OpJoinRandomRoom(); // joins any open room (no filter)
        }
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected(" + cause + ")");
        if (cause == DisconnectCause.DisconnectByClientLogic)
        {
            if (this.targetRegion != null)
            {
                this.lbc.ConnectToRegionMaster(this.targetRegion);
            }
        }
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
    }


    public void OnRegionListReceived(RegionHandler regionHandler)
    {
        Debug.Log("OnRegionListReceived: "+regionHandler.EnabledRegions.ToStringFull());

        regionHandler.PingMinimumOfRegions(this.OnRegionPingCompleted, null);
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
        // should be triggered when the room is full, by timeout or by user
        if (!string.IsNullOrEmpty(this.targetRoomName) && this.lbc.CloudRegion.StartsWith(this.targetRegion))
        {
            this.FinishRelocation();
            return;
        }

        if (this.RelocateOnRoomFull && this.lbc.CurrentRoom.PlayerCount == this.lbc.CurrentRoom.MaxPlayers)
        {
            this.RelocateMatch();
        }
    }


    public void OnJoinRoomFailed(short returnCode, string message)
    {
    }


    public void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed");
        this.lbc.OpCreateRoom(new EnterRoomParams() { RoomOptions = new RoomOptions() { MaxPlayers = 4 } });
    }


    public void OnLeftRoom()
    {
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
    }

    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        Debug.Log("OnRoomPropertiesUpdate");
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        Debug.Log("OnPlayerPropertiesUpdate");
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
    }
}