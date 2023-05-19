using System;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;


namespace Photon.Pun.UtilityScripts
{
    /// <summary>
    /// Unexpected disconnects recovery
    /// </summary>
    public class DisconnectsRecovery : MonoBehaviourPunCallbacks
    {
        [Tooltip("Whether or not attempt a rejoin without doing any checks.")]
        [SerializeField]
        private bool skipRejoinChecks;

        [Tooltip("Whether or not realtime webhooks are configured with persistence enabled")]
        [SerializeField]
        private bool persistenceEnabled;


        public bool DelayAction;

        private bool rejoinCalled;

        private int minTimeRequiredToRejoin = 0; // TODO: set dynamically based on PhotonNetwork.NetworkingClient.LoadBalancingPeer.RoundTripTime

        private DisconnectCause lastDisconnectCause;

        private bool reconnectCalled;

        private bool pendingAction;

        public bool ApplyAsyncSocket = true;

        public override void OnEnable()
        {
            base.OnEnable();


            if (this.ApplyAsyncSocket)
            {
                PhotonNetwork.NetworkingClient.LoadBalancingPeer.SocketImplementationConfig[ConnectionProtocol.Udp] = typeof(SocketUdpAsync);
                PhotonNetwork.NetworkingClient.LoadBalancingPeer.SocketImplementationConfig[ConnectionProtocol.Tcp] = typeof(SocketTcpAsync);
            }

            PhotonNetwork.NetworkingClient.StateChanged += this.OnStateChanged;
            PhotonNetwork.KeepAliveInBackground = 0f;
            PhotonNetwork.PhotonServerSettings.RunInBackground = false;
            Application.runInBackground = false;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.NetworkingClient.StateChanged -= this.OnStateChanged;
        }


        private void OnStateChanged(ClientState fromState, ClientState toState)
        {
            Debug.LogFormat("OnStateChanged from {0} to {1}, PeerState={2}", fromState, toState, PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState);
            //if (toState == ClientState.Disconnected)
            //{
            //    Debug.LogFormat("OnStateChanged from {0} to {1}, PeerState={2}", fromState, toState, 
            //        PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState);


            //    if (DelayAction)
            //    {
            //        Debug.Log("Pending action raised");
            //        pendingAction = true;
            //    }
            //    else
            //    {
            //        this.HandleDisconnect();
            //    }

            //}
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                PhotonNetwork.Disconnect();
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                PhotonNetwork.NetworkingClient.SimulateConnectionLoss(!PhotonNetwork.NetworkingClient.LoadBalancingPeer.IsSimulationEnabled);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("DebugSuppressSendingAcks is commented out. Needs a specific library build to support it.");
                //PhotonNetwork.NetworkingClient.LoadBalancingPeer.DebugSuppressSendingAcks = !PhotonNetwork.NetworkingClient.LoadBalancingPeer.DebugSuppressSendingAcks;
                //Debug.LogWarning("DebugSuppressSendingAcks: "+PhotonNetwork.NetworkingClient.LoadBalancingPeer.DebugSuppressSendingAcks);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                PhotonNetwork.Reconnect();
            }

            if (this.DelayAction && this.pendingAction)
            {
                this.pendingAction = false;
                Debug.Log("handle disconnect now");
                this.HandleDisconnect();
            }
        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogFormat("OnDisconnected(cause={0}) ClientState={1} PeerState={2}",
                            cause,
                            PhotonNetwork.NetworkingClient.State,
                            PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState);
            if (this.rejoinCalled)
            {
                Debug.LogError("Rejoin failed, client disconnected");
                this.rejoinCalled = false;
                return;
            }

            if (this.reconnectCalled)
            {
                Debug.LogError("Reconnect failed, client disconnected");
                this.reconnectCalled = false;
                return;
            }

            this.lastDisconnectCause = cause;

            if (PhotonNetwork.NetworkingClient.State == ClientState.Disconnected)
            {
                if (this.DelayAction)
                {
                    this.pendingAction = true;
                }
                else
                {
                    this.HandleDisconnect();
                }
            }
        }

        private void HandleDisconnect()
        {
            switch (this.lastDisconnectCause)
            {
                case DisconnectCause.None:
                case DisconnectCause.ServerTimeout:
                case DisconnectCause.Exception:
                case DisconnectCause.ClientTimeout:
                case DisconnectCause.DisconnectByServerLogic:
                case DisconnectCause.AuthenticationTicketExpired:
                case DisconnectCause.DisconnectByServerReasonUnknown:
                    //if (wasInRoom) 
                    //{ 
                    //    Debug.Log("CheckAndRejoin called");
                    //    this.CheckAndRejoin();
                    //}
                    //else
                    //{
                    //    Debug.Log("PhotonNetwork.Reconnect called");
                    //    reconnectCalled = PhotonNetwork.Reconnect();
                    //    Debug.Log("PhotonNetwork.Reconnect called result "+ reconnectCalled);
                    //}

                    bool reconnecting = false;
                    bool rejoining = PhotonNetwork.ReconnectAndRejoin();
                    if (!rejoining)
                    {
                        reconnecting = PhotonNetwork.Reconnect();
                    }

                    if (!rejoining && !reconnecting)
                    {
                        Debug.LogError("Should either rejoin or reconnect.");
                    }

                    break;
                case DisconnectCause.OperationNotAllowedInCurrentState:
                case DisconnectCause.CustomAuthenticationFailed:
                case DisconnectCause.DisconnectByClientLogic:
                case DisconnectCause.InvalidAuthentication:
                case DisconnectCause.ExceptionOnConnect:
                case DisconnectCause.MaxCcuReached:
                case DisconnectCause.InvalidRegion:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("cause", this.lastDisconnectCause, null);
            }

            this.lastDisconnectCause = DisconnectCause.None;
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            if (!this.rejoinCalled)
            {
                return;
            }

            this.rejoinCalled = false;
            Debug.LogErrorFormat("Quick rejoin failed with error code: {0} & error message: {1}", returnCode, message);
        }

        public override void OnJoinedRoom()
        {
            if (this.rejoinCalled)
            {
                Debug.Log("Rejoin successful");
                this.rejoinCalled = false;
            }
        }

        private void CheckAndRejoin()
        {
            if (this.skipRejoinChecks)
            {
                Debug.Log("PhotonNetwork.ReconnectAndRejoin called");
                this.rejoinCalled = PhotonNetwork.ReconnectAndRejoin();
            }
            else
            {
                bool wasLastActivePlayer = true;
                if (!this.persistenceEnabled)
                {
                    for (int i = 0; i < PhotonNetwork.PlayerListOthers.Length; i++)
                    {
                        if (!PhotonNetwork.PlayerListOthers[i].IsInactive)
                        {
                            wasLastActivePlayer = false;
                            break;
                        }
                    }
                }

                if ((PhotonNetwork.CurrentRoom.PlayerTtl < 0 || PhotonNetwork.CurrentRoom.PlayerTtl > this.minTimeRequiredToRejoin) // PlayerTTL checks
                    && (!wasLastActivePlayer || PhotonNetwork.CurrentRoom.EmptyRoomTtl > this.minTimeRequiredToRejoin || this.persistenceEnabled)) // EmptyRoomTTL checks
                {
                    Debug.Log("PhotonNetwork.ReconnectAndRejoin called");
                    this.rejoinCalled = PhotonNetwork.ReconnectAndRejoin();
                }
                else
                {
                    Debug.Log("PhotonNetwork.ReconnectAndRejoin not called, PhotonNetwork.Reconnect is called instead.");
                    this.reconnectCalled = PhotonNetwork.Reconnect();
                }
            }
        }

        public override void OnConnectedToMaster()
        {
            if (this.reconnectCalled)
            {
                Debug.Log("Reconnect successful");
                this.reconnectCalled = false;
            }
        }
    }
}