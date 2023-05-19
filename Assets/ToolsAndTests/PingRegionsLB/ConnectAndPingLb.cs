using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Photon.Realtime.Demo
{
    public class ConnectAndPingLb : MonoBehaviour, IConnectionCallbacks
    {
        public string AppId; // set in inspector
        private LoadBalancingClient lbc;

        private ConnectionHandler ch;
        public Text StateUiText;

        public void Start()
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


                Text uiText = this.StateUiText;
                string state = client.State.ToString();
                if (uiText != null && !uiText.text.Equals(state))
                {
                    uiText.text = "State: " + state;
                }
            }
        }


        public void OnConnected()
        {
        }

        public void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster");
            this.lbc.OpJoinRandomRoom();    // joins any open room (no filter)
        }

        public void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("OnDisconnected(" + cause + ")");
        }

        public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
        }

        public void OnCustomAuthenticationFailed(string debugMessage)
        {
        }

        public void OnRegionListReceived(RegionHandler regionHandler)
        {
            Debug.Log("OnRegionListReceived");
            regionHandler.PingMinimumOfRegions(this.OnRegionPingCompleted, null);
            this.lbc.Disconnect();
            this.lbc.LoadBalancingPeer.StopThread();
        }
        


        /// <summary>A callback of the RegionHandler, provided in OnRegionListReceived.</summary>
        /// <param name="regionHandler">The regionHandler wraps up best region and other region relevant info.</param>
        private void OnRegionPingCompleted(RegionHandler regionHandler)
        {
            Debug.Log("OnRegionPingCompleted " + regionHandler.BestRegion);
            Debug.Log("RegionPingSummary: " + regionHandler.SummaryToCache);
            
            //this.lbc.ConnectToRegionMaster(regionHandler.BestRegion.Code);
        }
    }
}