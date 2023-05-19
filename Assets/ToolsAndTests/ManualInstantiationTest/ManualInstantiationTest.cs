    using System.Collections.Generic;
    using ExitGames.Client.Photon;
    using UnityEngine;
    using Photon.Pun;
    using Photon.Realtime;

    public class ManualInstantiationTest : MonoBehaviour, IMatchmakingCallbacks, IOnEventCallback
    {
        public const byte InstantiateVrAvatarEventCode = 1; // example code, change to any value between 1 and 199

        public const string LocalClientPrefabName = "LocalPrefab";
        public const string RemoteClientPrefabName = "RemotePrefab";

        public void OnJoinedRoom()
        {
            GameObject localAvatar = Instantiate(Resources.Load(LocalClientPrefabName)) as GameObject;
            PhotonView photonView = localAvatar.GetComponent<PhotonView>();

            if (PhotonNetwork.AllocateViewID(photonView))
            {
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                {
                    CachingOption = EventCaching.AddToRoomCache,
                    Receivers = ReceiverGroup.Others
                };

                PhotonNetwork.RaiseEvent(InstantiateVrAvatarEventCode, photonView.ViewID, raiseEventOptions, SendOptions.SendReliable);
            }
            else
            {
                Debug.LogError("Failed to allocate a ViewId.");

                Destroy(localAvatar);
            }
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public void OnEvent(EventData photonEvent)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber != photonEvent.Sender)
            {
                if (photonEvent.Code == InstantiateVrAvatarEventCode)
                {
                    GameObject remoteAvatar = Instantiate(Resources.Load(RemoteClientPrefabName)) as GameObject;
                    PhotonView photonView = remoteAvatar.GetComponent<PhotonView>();
                    photonView.ViewID = (int) photonEvent.CustomData;
                }
            }
        }

        #region Unused IMatchmakingCallbacks

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
        }

        public void OnCreatedRoom()
        {
        }

        public void OnCreateRoomFailed(short returnCode, string message)
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

        #endregion
    }