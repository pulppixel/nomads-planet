// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonProfiler.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities,
// </copyright>
// <summary>
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

using ExitGames.Client.Photon;

namespace Photon.Pun.UtilityScripts
{
    public class PunProfiler : MonoBehaviour
    {

        public DropDownController GraphsDropDown;
        
        public Dictionary<string,MiniGraphControl> Graphs = new Dictionary<string, MiniGraphControl>();

        
        /// <summary>Option to turn collecting stats on or off (used in Update()).</summary>
        public bool statsOn = true;
        
        private void Start()
        {
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsEnabled = true;

       //     GraphOutgoing.SetName( "Outgoing Total packets (bytes)");
         //   GraphIncoming.SetName("Incoming Total packets (bytes)");
        }

        public void DeleteGraph(PunProfilerGraphPanel graph)
        {
            
        }
        
        /// <summary>Checks for shift+tab input combination (to toggle statsOn).</summary>
        public void Update()
        {

            bool statsToLog = false;
            TrafficStatsGameLevel gls = PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsGameLevel;
            long elapsedMs = PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsElapsedMs / 1000;
            if (elapsedMs == 0)
            {
                elapsedMs = 1;
            }


            string total = string.Format("Out {0,4} | In {1,4} | Sum {2,4}", gls.TotalOutgoingMessageCount, gls.TotalIncomingMessageCount, gls.TotalMessageCount);
            string elapsedTime = string.Format("{0}sec average:", elapsedMs);
            string average = string.Format("Out {0,4} | In {1,4} | Sum {2,4}", gls.TotalOutgoingMessageCount / elapsedMs, gls.TotalIncomingMessageCount / elapsedMs, gls.TotalMessageCount / elapsedMs);

            if (false)
            {
                GUILayout.BeginHorizontal();
                this.statsOn = GUILayout.Toggle(this.statsOn, "stats on");
                if (GUILayout.Button("Reset"))
                {
                    PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsReset();
                    PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsEnabled = true;
                }

                statsToLog = GUILayout.Button("To Log");
                GUILayout.EndHorizontal();
            }


            //   trafficStatsIn = "Incoming: \n" + PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsIncoming.ToString();
            // trafficStatsOut = "Outgoing: \n" + PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsOutgoing.ToString();
      //      GraphIncoming.AddValue(gls.TotalIncomingMessageCount / elapsedMs);
        //    GraphOutgoing.AddValue(gls.TotalOutgoingMessageCount / elapsedMs);


/*
            healthStats = string.Format(
                "ping: {6}[+/-{7}]ms resent:{8} \n\nmax ms between\nsend: {0,4} \ndispatch: {1,4} \n\nlongest dispatch for: \nev({3}):{2,3}ms \nop({5}):{4,3}ms",
                gls.LongestDeltaBetweenSending,
                gls.LongestDeltaBetweenDispatching,
                gls.LongestEventCallback,
                gls.LongestEventCallbackCode,
                gls.LongestOpResponseCallback,
                gls.LongestOpResponseCallbackOpCode,
                PhotonNetwork.NetworkingClient.LoadBalancingPeer.RoundTripTime,
                PhotonNetwork.NetworkingClient.LoadBalancingPeer.RoundTripTimeVariance,
                PhotonNetwork.NetworkingClient.LoadBalancingPeer.ResentReliableCommands);
                */

        }
    }
}