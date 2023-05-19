using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class InterestGroupsTests : MonoBehaviour, IPunInstantiateMagicCallback, IMatchmakingCallbacks
{
    [SerializeField]
    private byte interestGroup = 1;
    
    [ContextMenu("SubscribeToInterestGroup")]
    private void SubscribeToInterestGroup()
    {
        this.SubscribeToInterestGroup(this.interestGroup);
    }
    
    [ContextMenu("RaiseEventInGroup")]
    private void RaiseEventInGroup()
    {
        RaiseEventOptions options = new RaiseEventOptions();
        options.InterestGroup = interestGroup;
        PhotonNetwork.RaiseEvent(interestGroup, interestGroup, RaiseEventOptions.Default, SendOptions.SendReliable);
    }

    private void InstantiateInGroup(byte group)
    {
        PhotonNetwork.Instantiate("interestGroupPrefab", Vector3.zero, Quaternion.identity, group);
    }

    private void SubscribeToInterestGroup(byte group)
    {
        //PhotonNetwork.SetInterestGroups(new byte[0], new[] { group });
        //PhotonNetwork.SetInterestGroups(null, new byte[0]);
        PhotonNetwork.SetInterestGroups(group, true);
    }

    [ContextMenu("SubscribeToAllExistingInterestGroups")]
    private void SubscribeToAllExistingInterestGroups()
    {
        PhotonNetwork.SetInterestGroups(null, new byte[0]);
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
        PhotonNetwork.NetworkingClient.OpResponseReceived += NetworkingClientOnOpResponseReceived;
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClientOnEventReceived;
    }

    private void NetworkingClientOnEventReceived(EventData eventData)
    {
        if (eventData.Code == 200) // RPC
        {
            Debug.LogFormat("RPC event received {0}", eventData.ToStringFull());
        } 
        else if (eventData.Code == interestGroup)
        {
            Debug.LogFormat("Custom event {0} received on interest group {1}, {2}", eventData.Code, interestGroup, eventData.ToStringFull());
        }
    }

    private void NetworkingClientOnOpResponseReceived(OperationResponse operationResponse)
    {
        if (operationResponse.OperationCode == OperationCode.ChangeGroups)
        {
            Debug.LogFormat("OpChangeGroups response {0}", operationResponse.ToStringFull());
        } 
        else if (operationResponse.OperationCode == OperationCode.RaiseEvent)
        {
            Debug.LogFormat("OpRaiseEvent response {0}", operationResponse.ToStringFull());
        }
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        PhotonNetwork.NetworkingClient.OpResponseReceived -= NetworkingClientOnOpResponseReceived;
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClientOnEventReceived;
    }

    #region IMatchmakingCallbacks

    void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
    {
    }

    void IMatchmakingCallbacks.OnCreatedRoom()
    {
    }

    void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
    {
    }

    void IMatchmakingCallbacks.OnJoinedRoom()
    {
        this.SubscribeToInterestGroup(this.interestGroup);
        this.InstantiateInGroup(this.interestGroup);
        this.RaiseEventInGroup();
    }

    void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
    {
    }

    void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
    {
    }

    void IMatchmakingCallbacks.OnLeftRoom()
    {
    }

    #endregion

    #region IPunInstantiateMagicCallback
    
    void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
    {
    }

    #endregion
}
