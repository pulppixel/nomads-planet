using Photon.Pun;
using UnityEngine;

public class ViewIdRecycleTest : MonoBehaviourPunCallbacks {
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("viewIdRecycleTest", null, null);
    }

    public override void OnJoinedRoom()
    {
        int bound = PhotonNetwork.MAX_VIEW_IDS;
        for (int i = 0; i < bound; i++)
        {
            this.InstantiatePrefab();
        }
    }

    [ContextMenu("InstantiatePrefab")]
    private void InstantiatePrefab()
    {
        PhotonNetwork.InstantiateRoomObject("test", Vector3.zero, Quaternion.identity);
        PhotonNetwork.Instantiate("test", Vector3.zero, Quaternion.identity);
    }
}